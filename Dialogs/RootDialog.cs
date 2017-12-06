
namespace Microsoft.Bot.Sample.QnABot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    //using AdaptiveCards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    //using Util;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Text;

    [Serializable]
    [LuisModel("a286e61f-c49c-4195-a053-3829dd492d7f", "47a6f5b6ba8c47adbc094148721336a6")]
    public class RootDialog : LuisDialog<object>
    {
        private string category;
        private string severity;
        private string description;
        private string emailAddress;
        private string userName;
        private string userErrorDescription;
        private string requestType;


        public class Rootobject
        {
            public Properties properties { get; set; }
            public string type { get; set; }
        }



        public class Properties
        {


            public string emailaddress { get; set; }
            public string requestType { get; set; }
            public string userName { get; set; }
            public  string userErrorDescription{ get; set; }
        }


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"I'm sorry, I did not understand {result.Query}.\nType 'help' to know more about me :)");
            context.Done<object>(null);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm the PS Helpbot and I can help you make a request, submit issues or answer your questions\n" +
                                    "You can tell me things like _I need to reset my password_ or _I am seeing an error_ or _How do I upgrade my membership_.");
            context.Done<object>(null);
        }


        [LuisIntent("RequestService")]
        public async Task RequestService(IDialogContext context, LuisResult result)
        {
            //EntityRecommendation categoryEntityRecommendation;

            //result.TryFindEntity("category", out categoryEntityRecommendation);

            //this.category = ((Newtonsoft.Json.Linq.JArray)categoryEntityRecommendation?.Resolution["values"])?[0]?.ToString();
            //this.severity = "Low";
            //this.description = result.Query;

            //await this.EnsureInput(context);

            //var message = await argument;
            await context.PostAsync("Please enter your email address: ");
            this.requestType = "RequestService";

            PromptDialog.Text(context, this.eMailAddressReceivedAsync, "youremail@domain.com");
            


        }



        public async Task eMailAddressReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            this.emailAddress = await argument;
            await context.PostAsync($"Email Address: \"{this.emailAddress}\"");
            PromptDialog.Text(context, this.userNameReceiveAsync, "Please enter your username: ");


        }

        public async Task userNameReceiveAsync(IDialogContext context, IAwaitable<string> argument)
        {
            this.userName = await argument;
            await context.PostAsync($"Username: \"{this.userName}\"");
            PromptDialog.Text(context, this.userDescriptionAsync, "Please provide a short description of your request.");

        }

        public async Task userDescriptionAsync(IDialogContext context, IAwaitable<string> argument)
        {
            this.userErrorDescription = await argument;
            await context.PostAsync($"Issue: \"{this.userErrorDescription}\"");

            postHTTPAsync();
            context.Done<object>(null);
        }


        public async Task messageDisplay(IDialogContext context, IAwaitable<string> argument)
        {
            this.description = await argument;
            await context.PostAsync($"");
            context.Done<object>(null);
        }

        [LuisIntent("ReportIssue")]
        public async Task ReportIssue(IDialogContext context, LuisResult result)
        {
            EntityRecommendation categoryEntityRecommendation, severityEntityRecommendation;

            result.TryFindEntity("category", out categoryEntityRecommendation);
            result.TryFindEntity("severity", out severityEntityRecommendation);

            this.category = ((Newtonsoft.Json.Linq.JArray)categoryEntityRecommendation?.Resolution["values"])?[0]?.ToString();
            this.severity = ((Newtonsoft.Json.Linq.JArray)severityEntityRecommendation?.Resolution["values"])?[0]?.ToString();
            this.description = result.Query;

            await this.EnsureInput(context);
        }

        [LuisIntent("Query")]
        public async Task Query(IDialogContext context, LuisResult result)
        {
            EntityRecommendation categoryEntityRecommendation;

            result.TryFindEntity("category", out categoryEntityRecommendation);

            this.category = ((Newtonsoft.Json.Linq.JArray)categoryEntityRecommendation?.Resolution["values"])?[0]?.ToString();
            this.severity = "Low";
            this.description = result.Query;

            await this.EnsureInput(context);
        }

        private async Task EnsureInput(IDialogContext context)
        {
            if (this.severity == null)
            {
                var severities = new string[] { "high", "normal", "low" };
                PromptDialog.Choice(context, this.SeverityMessageReceivedAsync, severities, "Which is the severity of this issue?");
            }
            else if (this.category == null)
            {
                PromptDialog.Text(context, this.CategoryMessageReceivedAsync, "Which would be the category for this (issue, request, query)?");
            }
            else
            {
                //For modification
                var text = $"Great! I'm going to create a **{this.severity}** severity ticket in the **{this.category}** category. " +
                        $"The description I will use is _\"{this.description}\"_. Can you please confirm that this information is correct?";

                PromptDialog.Confirm(context, this.IssueConfirmedMessageReceivedAsync, text);
            }
        }

        public async Task SeverityMessageReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            this.severity = await argument;
            await this.EnsureInput(context);
        }

        public async Task CategoryMessageReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            this.category = await argument;
            await this.EnsureInput(context);
        }

        public async Task IssueConfirmedMessageReceivedAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirmed = await argument;

            if (confirmed)
            {
                //For modification
                await context.PostAsync("Feature coming soon");
            }
            else
            {
                await context.PostAsync("Ok. The ticket was not created. You can start again if you want.");
            }

            context.Done<object>(null);
        }

        public async Task postHTTPAsync()
        {
            var json = new Rootobject();
            json.properties = new Properties();

            //json.properties.userName = new Username();
            json.properties.userName = this.userName;
            //json.properties.userName.type = "string";

            //json.properties.emailaddress = new Emailaddress();
            json.properties.emailaddress = this.emailAddress;
            //json.properties.emailaddress.type = "string";

            //json.properties.userErrorDescription = new UserErrorDescription();
            json.properties.userErrorDescription = this.userErrorDescription;
            json.properties.requestType = this.requestType;
            //json.properties.userErrorDescription.type = "string";

            string jsonStr = JsonConvert.SerializeObject(json);

            //string jsonStr = "{'userName': 'baeron','emailAddress':'takjinn@gmail.com','userErrorDescription': 'test'}";

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://prod-07.centralus.logic.azure.com:443/workflows/11a213e38ae24f729a4f86e5b01175d8/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=wImLtC3KNBuOVqGa-7c-fODZe_XfId2eY2P1TyPawp0", content);
                
            }
        }

    }
}
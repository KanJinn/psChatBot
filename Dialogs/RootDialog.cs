﻿
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

    [Serializable]
    [LuisModel("a286e61f-c49c-4195-a053-3829dd492d7f", "47a6f5b6ba8c47adbc094148721336a6")]
    public class RootDialog : LuisDialog<object>
    {
        private string category;
        private string severity;
        private string description;

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
            EntityRecommendation categoryEntityRecommendation;

            result.TryFindEntity("category", out categoryEntityRecommendation);

            this.category = ((Newtonsoft.Json.Linq.JArray)categoryEntityRecommendation?.Resolution["values"])?[0]?.ToString();
            this.severity = "Low";
            this.description = result.Query;

            await this.EnsureInput(context);
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
    }
}
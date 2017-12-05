using System;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using System.Diagnostics;

namespace Microsoft.Bot.Sample.QnABot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // check if activity is of type message
            if (activity.GetActivityType() == ActivityTypes.Message)
            {
                //await Conversation.SendAsync(activity, () => new BasicQnAMakerDialog());
                await Conversation.SendAsync(activity, () => new RootDialog());


            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        //public async Task DescriptionMessageReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        //{
        //    this.description = await argument;
        //    await this.EnsureInput(context);
        //    //var severities = new string[] { "high", "normal", "low" };
        //    //PromptDialog.Choice(context, this.SeverityMessageReceivedAsync, severities, "Which is the severity/urgency of this request?");
        //}

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                //ConnectorClient client = new ConnectorClient(new Uri(message.ServiceUrl));

                //var reply = message.CreateReply();

                //reply.Text = "Hello user how are you?";

                //await client.Conversations.ReplyToActivityAsync(reply);

            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
                //if (message.Action == "add")
                //{
                //    var reply = message.CreateReply("WELCOME!!!");
                //    ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                //    await connector.Conversations.ReplyToActivityAsync(reply);
                //}
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}
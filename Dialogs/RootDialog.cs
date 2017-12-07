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
using System.Threading;
using QnABot.Dialogs;

namespace Microsoft.Bot.Sample.QnABot
{
    

    
        [Serializable]
        public class RootDialog : IDialog<object>
        {
            public Task StartAsync(IDialogContext context)
            {
                context.Wait(MessageReceivedAsync);
                return Task.CompletedTask;
            }
            private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
            {
                var activity = await result as Activity;
                await context.Forward(new LuisDialog(), ResumeAftelLuisDialog, activity, CancellationToken.None);
            }
            private async Task ResumeAftelLuisDialog(IDialogContext context, IAwaitable<object> result)
            {
                context.Wait(MessageReceivedAsync);
            }
        }


    


}
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IgorekBot.Helpers
{
    public static class MenuHelper
    {
        public static IMessageActivity CreateMenu(IDialogContext context, List<string> buttons, string text = null)
        {
            var reply = context.MakeMessage();
            reply.Text = text;

            reply.SuggestedActions = new SuggestedActions
            {
                Actions = buttons.Select(b => new CardAction(ActionTypes.PostBack, b, value: b)).ToList()
            };

            return reply;
        }
    }
}
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using IgorekBot.Models;

namespace IgorekBot.Helpers
{
    public static class MenuHelper
    {
        public static IMessageActivity CreateMenu(IDialogContext context, IEnumerable<string> buttons, string text = null)
        {
            var reply = context.MakeMessage();
            reply.Text = text;

            reply.SuggestedActions = new SuggestedActions
            {
                Actions = buttons.Select(b => new CardAction(ActionTypes.PostBack, b, value: b)).ToList()
            };

            return reply;
        }

        public static IMessageActivity CreateMainMenuMessage(IDialogContext context, KeyboardButton[][] buttons, string text = null, bool isInlineKeyboard = false)
        {
            var message = context.MakeMessage();
            var keyboard = new StringBuilder();
            foreach (var row in buttons)
            {
                string rowKeyboard = "";
                var first = true;
                foreach (var cardAction in row)
                {
                    if (!first)
                    {
                        rowKeyboard += ",";
                    }
                    first = false;
                    rowKeyboard += string.Format("{{text: \"" + cardAction.Text + "\", callback_data: \"" +
                                                cardAction.Value + "\"}}");
                }
                keyboard.Append("[").Append(rowKeyboard).Append("],");
            }

            var keyboardType = "keyboard";
            if (isInlineKeyboard)
            {
                keyboardType = "inline_keyboard";
            }

            message.ChannelData = $@"
{{
    ""method"": ""sendMessage"",
    ""parameters"": {{
        ""text"": ""{text}"",
        ""parse_mode"": ""Markdown"",
        ""reply_markup"": {{
            ""resize_keyboard"": true,
            ""{keyboardType}"": [
                       {keyboard}
                   ]
        }}
    }}
}}";
            return message;
        }
    }
}
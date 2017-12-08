using Microsoft.Bot.Builder.Dialogs.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Mail;
using IgorekBot.Properties;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class PromptEmail : Prompt<string, string>
    {

        public PromptEmail(string prompt, string retry = null, string tooManyAttempts = null, int attempts = 3, bool showMenu = true) : base(new PromptOptions<string>(prompt, retry, tooManyAttempts, attempts: attempts))
        {
        }

        protected override bool TryParse(IMessageActivity message, out string result)
        {
            var quitCondition = message.Text.Equals(Resources.CancelCommand, StringComparison.InvariantCultureIgnoreCase);
            var validEmail = IsValidEmail(message.Text);

            result = validEmail ? message.Text : null;

            return validEmail || quitCondition;
        }
        
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
using System;
using System.Net.Mail;
using IgorekBot.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class PromptEmail : Prompt<string, string>
    {
        public PromptEmail(string prompt, string retry = null, string tooManyAttempts = null, int attempts = 3,
            bool showMenu = true) : base(new PromptOptions<string>(prompt, retry, tooManyAttempts, attempts: attempts))
        {
        }

        protected override bool TryParse(IMessageActivity message, out string result)
        {
            var quitCondition =
                message.Text.Equals(Resources.CancelCommand, StringComparison.InvariantCultureIgnoreCase);
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
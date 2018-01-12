using System;
using IgorekBot.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class PromptDateTime : Prompt<DateTime, DateTime>
    {
        public PromptDateTime(string prompt, string retry = null, string tooManyAttempts = null, int attempts = 3) :
            base(new PromptOptions<DateTime>(prompt, retry, tooManyAttempts, attempts: attempts))
        {
        }

        protected override bool TryParse(IMessageActivity message, out DateTime result)
        {
            var quitCondition = message.Text.Equals(Resources.BackCommand, StringComparison.InvariantCultureIgnoreCase);
            DateTime dt;
            var isValid = DateTime.TryParse(message.Text, out dt);

            result = isValid ? dt : DateTime.MinValue;

            return isValid || quitCondition;
        }
    }
}
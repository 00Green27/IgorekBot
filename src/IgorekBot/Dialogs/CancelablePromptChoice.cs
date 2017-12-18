using System;
using System.Collections.Generic;
using System.Linq;
using IgorekBot.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class CancelablePromptChoice<T> : PromptDialog.PromptChoice<T>
    {
        private static readonly IEnumerable<string> _cancelTerms = new[]
            {"Отмена", "Назад", "О", "Назад", Resources.BackCommand};

        private readonly CancelablePromptOptions<T> _promptOptions;

        public CancelablePromptChoice(CancelablePromptOptions<T> promptOptions)
            : base(promptOptions)
        {
            _promptOptions = promptOptions;
        }

        public CancelablePromptChoice(IEnumerable<T> options, string prompt, string cancelPrompt, string retry,
            int attempts, PromptStyle promptStyle = PromptStyle.Auto, IEnumerable<string> descriptions = null)
            : this(new CancelablePromptOptions<T>(prompt, cancelPrompt, retry, options: options.ToList(),
                attempts: attempts, promptStyler: new PromptStyler(promptStyle), descriptions: descriptions?.ToList()))
        {
        }

        public static void Choice(IDialogContext context, ResumeAfter<T> resume, IEnumerable<T> options, string prompt,
            string cancelPrompt = null, string retry = null, int attempts = 3,
            PromptStyle promptStyle = PromptStyle.Auto, IEnumerable<string> descriptions = null)
        {
            Choice(context, resume,
                new CancelablePromptOptions<T>(prompt, cancelPrompt, retry, options: options.ToList(),
                    attempts: attempts, promptStyler: new PromptStyler(promptStyle),
                    descriptions: descriptions?.ToList()));
        }

        public static void Choice(IDialogContext context, ResumeAfter<T> resume,
            CancelablePromptOptions<T> promptOptions)
        {
            var child = new CancelablePromptChoice<T>(promptOptions);
            context.Call(child, resume);
        }

        public static bool IsCancel(string text)
        {
            return _cancelTerms.Any(t => string.Equals(t, text, StringComparison.CurrentCultureIgnoreCase));
        }

        protected override bool TryParse(IMessageActivity message, out T result)
        {
            if (IsCancel(message.Text))
            {
                result = default(T);
                return true;
            }

            return base.TryParse(message, out result);
        }

        protected override IMessageActivity MakePrompt(IDialogContext context, string prompt,
            IReadOnlyList<T> options = null, IReadOnlyList<string> descriptions = null, string speak = null)
        {
            if (!string.IsNullOrEmpty(_promptOptions.CancelPrompt))
                prompt += Environment.NewLine + _promptOptions.CancelPrompt;
            //prompt += Environment.NewLine + (_promptOptions.CancelPrompt ?? _promptOptions.DefaultCancelPrompt);
            return base.MakePrompt(context, prompt, options, descriptions);
        }
    }

    [Serializable]
    public class CancelablePromptOptions<T> : PromptOptions<T>
    {
        public CancelablePromptOptions(string prompt, string cancelPrompt = null, string retry = null,
            string tooManyAttempts = null, IReadOnlyList<T> options = null, int attempts = 3,
            PromptStyler promptStyler = null, IReadOnlyList<string> descriptions = null)
            : base(prompt, retry, tooManyAttempts, options, attempts, promptStyler, descriptions)
        {
            DefaultCancelPrompt = Resources.CancelablePromptChoice_CancelText;

            CancelPrompt = cancelPrompt;
        }

        public string DefaultCancelPrompt { get; }

        public string CancelPrompt { get; }
    }
}
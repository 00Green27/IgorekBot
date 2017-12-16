using System;
using System.Threading;
using System.Threading.Tasks;
using IgorekBot.Properties;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;

namespace IgorekBot.Dialogs
{
    public class ResetScorable : IScorable<IActivity, double>
    {
        private readonly IDialogTask _task;

        public ResetScorable(IDialogTask task)
        {
            SetField.NotNull(out _task, nameof(task), task);
        }

        public Task DoneAsync(IActivity item, object state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public double GetScore(IActivity item, object state)
        {
            return state != null ? 1 : 0;
        }

        public bool HasScore(IActivity item, object state)
        {
            return state != null;
        }

        public async Task PostAsync(IActivity item, object state, CancellationToken token)
        {
            _task.Reset();
        }

        public async Task<object> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
                if (message.Text.Equals("/reset", StringComparison.InvariantCultureIgnoreCase))
                    return message.Text;

            return null;
        }
    }
}
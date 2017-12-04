using Microsoft.Bot.Builder.Scorables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using IgorekBot.Properties;

namespace IgorekBot.Dialogs
{
    public class CancelScorable : IScorable<IActivity, double>
    {
        private readonly IDialogTask _task;
        
        public CancelScorable(IDialogTask task)
        {
            SetField.NotNull(out this._task, nameof(task), task);
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

            if(message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if(message.Text.Equals(Resources.CancelCommand, StringComparison.InvariantCultureIgnoreCase))
                {
                    return message.Text;
                }
            }

            return null;
        }
    }
}
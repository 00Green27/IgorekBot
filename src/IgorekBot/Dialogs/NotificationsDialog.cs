using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Internals.Fibers;
using IgorekBot.BLL.Services;
using IgorekBot.Data.Models;
using Microsoft.Bot.Connector;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class NotificationsDialog : IDialog<object>
    {
        private readonly IBotService _botSvc;
        private readonly ITimeSheetService _timeSheetSvc;
        private UserProfile _profile;

        public NotificationsDialog(IBotService botSvc, ITimeSheetService timeSheetSvc)
        {
            SetField.NotNull(out _botSvc, nameof(botSvc), botSvc);
            SetField.NotNull(out _timeSheetSvc, nameof(timeSheetSvc), timeSheetSvc);
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.UserData.TryGetValue(@"profile", out _profile);
            var cookie = _botSvc.GetCookieAsync(_profile);
            var msg = "Хотите получать оповещения?";
            if(cookie == null)
            {
                msg = "Хотите отключить оповещения?";
            }
            PromptDialog.Confirm(context, AfterNotificationsSelected, msg);
        }

        private async Task AfterNotificationsSelected(IDialogContext context, IAwaitable<bool> result)
        {
            var confirm = await result;
            if (confirm)
            {
                //await _botSvc.GetCookieAsync();
            }
            await context.PostAsync($"Хорошо.");
            context.Done(true);
        }
    }
}
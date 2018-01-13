using System;
using System.Threading.Tasks;
using IgorekBot.BLL.Services;
using IgorekBot.Data.Models;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class NotificationsDialog : IDialog<object>
    {
        private readonly IBotService _botSvc;
        private readonly ITimeSheetService _timeSheetSvc;
        private bool _doSubscribe;
        private UserProfile _profile;

        public NotificationsDialog(IBotService botSvc, ITimeSheetService timeSheetSvc)
        {
            SetField.NotNull(out _botSvc, nameof(botSvc), botSvc);
            SetField.NotNull(out _timeSheetSvc, nameof(timeSheetSvc), timeSheetSvc);
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.UserData.TryGetValue(@"profile", out _profile);
            var reference = _botSvc.GetConversationReference(_profile);
            var msg = "Хотите отключить оповещения?";
            if (reference == null)
            {
                _doSubscribe = true;
                msg = "Хотите получать оповещения?"; 
            }
            PromptDialog.Confirm(context, AfterNotificationsSelected, msg);
        }

        private async Task AfterNotificationsSelected(IDialogContext context, IAwaitable<bool> result)
        {
            var confirm = await result;
            if (confirm)
            {
                if (_doSubscribe)
                {
                    var conversationRef = context.Activity.ToConversationReference();
                    var encode = UrlToken.Encode(conversationRef);
                    await _botSvc.SaveConversationReference(_profile, encode);
                }
                else
                {
                    await _botSvc.RemoveConversationReference(_profile);
                }
            }
            await context.PostAsync($"Хорошо.");
            context.Done(true);
        }
    }
}
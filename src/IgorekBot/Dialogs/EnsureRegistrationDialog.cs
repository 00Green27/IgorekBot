using Microsoft.Bot.Builder.Dialogs;
using NMSService.NMSServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Internals.Fibers;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Data.Models;
using IgorekBot.Properties;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class EnsureRegistrationDialog : IDialog<UserProfile>
    {
        private readonly ITimeSheetService _service;
        private UserProfile _profile;

        public EnsureRegistrationDialog(ITimeSheetService service)
        {
            SetField.NotNull(out _service, nameof(service), service);
        }

        public Task StartAsync(IDialogContext context)
        {
            EnsureRegistration(context);
            return Task.CompletedTask;
        }

        private void EnsureRegistration(IDialogContext context)
        {

            if (!context.UserData.TryGetValue(@"profile", out _profile))
            {
                _profile = new UserProfile();
                var response = _service.GetUserById(new GetUserByIdRequest { ChannelId = context.MakeMessage().From.Id });

                if (response.Result == 1)
                {
                    var promptPhoneDialog = new PromptEmail(Resources.AuthenticationDialog_EMail_Prompt);
                    context.Call(promptPhoneDialog, this.ResumeAfterEmailEntered);
                }
                else
                {
                    //context.Done(response.Employee.Select(e => new UserProfile { FirstName = e.FirstName, LastName = e.LastName, UserId = e.No}));
                    context.Done(new object());
                }
            }
            else
            {
                context.Done(_profile);
            }
        }

        private async Task ResumeAfterEmailEntered(IDialogContext context, IAwaitable<string> result)
        {
            _profile.Email = await result;

        }
    }
}
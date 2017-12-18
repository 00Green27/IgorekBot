using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Threading.Tasks;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Data.Models;
using IgorekBot.Helpers;
using IgorekBot.Properties;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class RegistrationDialog : IDialog<UserProfile>
    {
        private readonly ITimeSheetService _timeSheetSvc;
        private UserProfile _profile;

        public RegistrationDialog(ITimeSheetService timeSheetSvc)
        {
            SetField.NotNull(out _timeSheetSvc, nameof(timeSheetSvc), timeSheetSvc);
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(MenuHelper.CreateMenu(context, new List<string> { Resources.CancelCommand }, Resources.AuthenticationDialog_EMail_Prompt));
            context.Wait(ResumeAfterEmailEntered);
        }

        private async Task ResumeAfterEmailEntered(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (IsValidEmail(message.Text))
            {
                var response = _timeSheetSvc.AddUserByEmail(new AddUserByEmailRequest { Email = message.Text });
                if (response.Result == 1)
                {
                    context.Fail(new Exception(response.ErrorText));
                }
                else
                {
                    _profile = new UserProfile { Email = message.Text, UserId = message.From.Id, UserName = message.From.Name };
                    await context.PostAsync(String.Format(Resources.AuthenticationDialog_Code_Prompt, response.FirstName));
                    context.Wait(ResumeAfterActivationCodeEntered);
                }
            }
            else if (message.Text.Equals(Resources.CancelCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Done<UserProfile>(null);
            }
            else
            {
                await context.PostAsync(Resources.AuthenticationDialog_EMail_Prompt);
                context.Wait(ResumeAfterEmailEntered);
            }

        }

        private async Task ResumeAfterActivationCodeEntered(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            
            if (message.Text.Equals(Resources.CancelCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Done<UserProfile>(null);
            }

            var response = _timeSheetSvc.ValidatePassword(new ValidatePasswordRequest
            {
                ChannelId = _profile.UserId,
                Email = _profile.Email,
                Password = message.Text.Trim()
            });

            if (response.Result == 1)
            {
                context.Fail(new Exception(response.ErrorText));
            }
            else
            {
                _profile.FirstName = response.Employee.FirstName;
                _profile.LastName = response.Employee.LastName;
                _profile.EmployeeNo = response.Employee.No;
                context.Done(_profile);
            }
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
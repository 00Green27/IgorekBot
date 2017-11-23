using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IgorekBot.BLL.Interfaces;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Common;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class AuthenticationDialog : IDialog<object>
    {
        private readonly ITimeSheetService _timeSheetService;
        private string _email;
        private string _userId;

        public AuthenticationDialog()
        {
            _timeSheetService = new TimeSheetService();
            _userId = Common.CommonConversation.CurrentActivity?.From?.Id;
        }
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedStart);

            return Task.CompletedTask;
        }

        public async Task MessageReceivedStart(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            await context.PostAsync("Укажите рабочий email");

            context.Wait(MessageReceivedEmailRegistration);
        }

        public async Task MessageReceivedEmailRegistration(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (IsValidEmail(message.Text))
            {
                var response = _timeSheetService.AddUserByEMail(new AddUserByEMailRequest {ChannelType = (int) ChannelTypes.Telegram, EMail = message.Text});
                if (response.Result == 1)
                {
                    await context.PostAsync(response.ErrorText);
                    context.Wait(MessageReceivedStart);
                }
                else
                {
                    _email = message.Text;
                    await context.PostAsync($"{response.FirstName}, введите код активации");
                    context.Wait(MessageReceivedActivationCode);
                }

            }
            else
            {
                context.Wait(MessageReceivedStart);
            }
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task MessageReceivedActivationCode(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var code = await argument;
            var userId = code.From?.Id;
            var response = _timeSheetService.ValidatePassword(new ValidatePasswordRequest {ChannelType = (int)ChannelTypes.Telegram, ChannelId = userId, EMail = _email, Password = code.Text.Trim()});

            if (response.Result == 1)
            {
                await context.PostAsync(response.ErrorText);
            }
            else
            {
                await context.PostAsync($"Приветствую, {response.XMLPort.Employee.FirstOrDefault()?.FirstName}");
            }

            context.Wait(MessageReceivedActivationCode);
        }
    }
}
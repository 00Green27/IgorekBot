using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IgorekBot.BLL.Interfaces;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Common;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using NMSService.NMSServiceReference;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class AuthenticationDialog : IDialog<Employee>
    {
        private readonly ITimeSheetService _timeSheetService;
        private string _email;
        private string _userId;

        public AuthenticationDialog()
        {
            _timeSheetService = new TimeSheetService();
        }
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedStart);

            return Task.CompletedTask;
        }

        public async Task MessageReceivedStart(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {

            var message = await argument;

            var response = _timeSheetService.GetUserById(new GetUserByIdRequest
            {
                ChannelType = (int)ChannelTypes.Telegram,
                ChannelId = message?.From?.Id
            });

            if (response.Result == 1)
            {
                await context.PostAsync(CreateMessage(context, "Укажите рабочий email"));
                context.Wait(MessageReceivedEmailRegistration);
            }
            else
            {
                context.Done(response.XMLPort.Employee.First());
            }
        }

        public static IMessageActivity CreateMessage(IDialogContext context, string text)
        {

            var message = context.MakeMessage();
            message.Text = text;
            message.Type = ActivityTypes.Message;
            message.TextFormat = TextFormatTypes.Plain;
            message.SuggestedActions = new SuggestedActions
            {
                Actions = new List<CardAction>
                {
                    new CardAction { Title = "Отмена", Type=ActionTypes.ImBack, Value="/cancelRegistration" },
                }
            };
            return message;
        }

        public async Task MessageReceivedEmailRegistration(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (IsValidEmail(message.Text))
            {
                var response = _timeSheetService.AddUserByEMail(new AddUserByEMailRequest {ChannelType = (int) ChannelTypes.Telegram, EMail = message.Text});
                if (response.Result == 1)
                {
                    await context.PostAsync(CreateMessage(context, response.ErrorText));
                    context.Wait(MessageReceivedStart);
                }
                else
                {
                    _email = message.Text;
                    await context.PostAsync(CreateMessage(context, $"{response.FirstName}, введите код активации"));
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
                context.Done(response.XMLPort.Employee.First());
            }

            context.Wait(MessageReceivedActivationCode);
        }
    }
}
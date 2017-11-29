using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
    public class RegistrationDialog : IDialog<Employee>
    {
        private readonly ITimeSheetService _timeSheetService;
        private string _email;
        private string _userId;

        public RegistrationDialog()
        {
            _timeSheetService = new TimeSheetService();
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(CreateMessage(context, "Укажите рабочий email"));

            context.Wait(MessageReceivedEmailRegistration);
        }

        public async Task MessageReceivedStart(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            var response = _timeSheetService.GetUserById(new GetUserByIdRequest
            {
                ChannelType = (int) ChannelTypes.Telegram,
                ChannelId = message?.From?.Id
            });

            if (response.Result == 1)
                context.Wait(MessageReceivedEmailRegistration);
            else
                context.Done(response.XMLPort.Employee.First());
        }


        public async Task MessageReceivedEmailRegistration(IDialogContext context,
            IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;


            if (message.Text.ToLower().Equals("/cancel", StringComparison.InvariantCultureIgnoreCase))
            {
                context.Done(new Employee());
            }
            else if (IsValidEmail(message.Text))
            {
                var response = _timeSheetService.AddUserByEMail(new AddUserByEMailRequest
                {
                    ChannelType = (int) ChannelTypes.Telegram,
                    EMail = message.Text
                });
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
                await context.PostAsync(CreateMessage(context, "Укажите рабочий email"));
                context.Wait(MessageReceivedEmailRegistration);
            }
        }


        public async Task MessageReceivedActivationCode(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var code = await argument;
            var userId = code.From?.Id;
            var response = _timeSheetService.ValidatePassword(new ValidatePasswordRequest
            {
                ChannelType = (int) ChannelTypes.Telegram,
                ChannelId = userId,
                EMail = _email,
                Password = code.Text.Trim()
            });

            if (response.Result == 1)
                await context.PostAsync(response.ErrorText);
            else
                context.Done(response.XMLPort.Employee.First());

            context.Wait(MessageReceivedActivationCode);
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
                    new CardAction {Title = "Отмена", Type = ActionTypes.ImBack, Value = "/cancel"}
                }
            };
            return message;
        }

        private bool IsValidEmail(string email)
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
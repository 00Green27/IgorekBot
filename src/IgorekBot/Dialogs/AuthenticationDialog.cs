using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using NMSService.NMSServiceReference;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class AuthenticationDialog : IDialog<Employee>
    {
        private readonly ITimeSheetService _service;
        private string _email;

        public AuthenticationDialog(ITimeSheetService service)
        {
            SetField.NotNull(out _service, nameof(service), service);
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(CreateReply(context, Resources.AuthenticationDialog_EMail_Prompt));

            context.Wait(MessageReceivedEmailRegistration);
        }

        public async Task MessageReceivedStart(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            var response = _service.GetUserById(new GetUserByIdRequest
            {
                ChannelId = message?.From?.Id
            });

            if (response.Result == 1)
                context.Wait(MessageReceivedEmailRegistration);
            else
                context.Done(response.Employee);
        }


        public async Task MessageReceivedEmailRegistration(IDialogContext context,
            IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (IsValidEmail(message.Text))
            {
                var response = _service.AddUserByEmail(new AddUserByEmailRequest {Email = message.Text});

                if (response.Result == 1)
                {
                    await context.PostAsync(CreateReply(context, response.ErrorText));
                    context.Done(new Employee());
                }
                else
                {
                    _email = message.Text;
                    await context.PostAsync(CreateReply(context,
                        string.Format(Resources.AuthenticationDialog_Code_Prompt, response.FirstName)));
                    context.Wait(MessageReceivedActivationCode);
                }
            }
            else
            {
                await context.PostAsync(CreateReply(context, Resources.AuthenticationDialog_EMail_Prompt));
                context.Wait(MessageReceivedEmailRegistration);
            }
        }

        public async Task MessageReceivedActivationCode(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var code = await argument;
            var userId = code.From?.Id;
            var response = _service.ValidatePassword(new ValidatePasswordRequest
            {
                ChannelId = userId,
                Email = _email,
                Password = code.Text.Trim()
            });

            if (response.Result == 1)
                await context.PostAsync(response.ErrorText);
            else
                context.Done(response.Employee);

            context.Wait(MessageReceivedActivationCode);
        }

        public static IMessageActivity CreateReply(IDialogContext context, string text)
        {
            var message = context.MakeMessage();
            message.Text = text;
            message.Type = ActivityTypes.Message;
            message.TextFormat = TextFormatTypes.Plain;
            message.SuggestedActions = new SuggestedActions
            {
                Actions = new List<CardAction>
                {
                    new CardAction {Title = Resources.CancelCommand, Type = ActionTypes.PostBack}
                }
            };
            return message;
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
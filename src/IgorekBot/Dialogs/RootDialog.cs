using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using NMSService.NMSServiceReference;
using Microsoft.Bot.Builder.Internals.Fibers;
using Resources = IgorekBot.Properties.Resources;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly ITimeSheetService _service;

        public RootDialog(ITimeSheetService service)
        {
            SetField.NotNull(out _service, nameof(service), service);
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            var response = _service.GetUserById(new GetUserByIdRequest { ChannelId = message?.From?.Id });
            if (response.Result == 1 || message.Text.Equals(Resources.RegistrationCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call(new AuthenticationDialog(_service), AuthenticationDialogResumeAfter);
            }
            else if (message.Text.Equals(Resources.TimeSheetCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                //await context.Forward(new TimeSheetDialog(_service), TimeSheetDialogResumeAfterAsync, message);
                context.Call(new TimeSheetDialog(_service), TimeSheetDialogResumeAfterAsync);
            }
            else if (message.Text.Equals(Resources.EnterAbsenceCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call(new EnterAbsenceDialog(), EnterAbsenceDialogResumeAfterAsync);
            } 
            else
            {
                await context.PostAsync(CreateReply(context, Resources.RootDialog_Didnt_Understand_Message));
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task TimeSheetDialogResumeAfterAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(CreateReply(context, Resources.RootDialog_Main_Message));
            context.Wait(MessageReceivedAsync);
        }

        private async Task EnterAbsenceDialogResumeAfterAsync(IDialogContext context, IAwaitable<object> result)
        {
            var employee = await result;
            context.Wait(MessageReceivedAsync);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
        }
        
        private async Task AuthenticationDialogResumeAfter(IDialogContext context, IAwaitable<Employee> result)
        {
            var employee = await result;
            if(employee.FirstName != null)
            {
                context.UserData.SetValue(@"profile", employee);
                await context.PostAsync(CreateReply(context, string.Format(Resources.RootDialog_Welcome_Message, employee.FirstName)));
            }
            else
            {
                var reply = context.MakeMessage();
                reply.Text = Resources.RootDialog_Authentication_Message;
                reply.Type = ActivityTypes.Message;
                reply.TextFormat = TextFormatTypes.Plain;

                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>
                    {
                        new CardAction { Title = Resources.RegistrationCommand, Type=ActionTypes.PostBack, Value = Resources.RegistrationCommand  },
                    }
                };

                await context.PostAsync(reply);
            }

        }
        
        public static IMessageActivity CreateReply(IDialogContext context, string text)
        {
            var reply = context.MakeMessage();
            reply.Text = text;
            reply.Type = ActivityTypes.Message;
            reply.TextFormat = TextFormatTypes.Plain;

            reply.SuggestedActions = new SuggestedActions
            {
                Actions = new List<CardAction>
                    {
                        new CardAction { Title = Resources.TimeSheetCommand, Type=ActionTypes.PostBack, Value = Resources.TimeSheetCommand },
                        new CardAction { Title = Resources.EnterAbsenceCommand, Type=ActionTypes.PostBack, Value = Resources.TimeSheetCommand },
                    }
            };

            return reply;
        }
    }
}
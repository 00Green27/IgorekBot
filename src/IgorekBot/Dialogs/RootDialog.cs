using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Threading.Tasks;
using IgorekBot.BLL.Interfaces;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Common;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Resource;
using Microsoft.Bot.Connector;
using NMSService.NMSServiceReference;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly ITimeSheetService _timeSheetService;

        public RootDialog()
        {
            _timeSheetService = new TimeSheetService();
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {

            var message = await argument;
            if (message.Text.ToLower().Equals("/start", StringComparison.InvariantCultureIgnoreCase))
            {
                var response = _timeSheetService.GetUserById(new GetUserByIdRequest
                {
                    ChannelType = (int)ChannelTypes.Telegram,
                    ChannelId = message?.From?.Id
                });

                if (response.Result == 1)
                {
                    var reply = context.MakeMessage();
                    reply.Text = "Вам необходимо зарегистрироваться";
                    reply.Type = ActivityTypes.Message;
                    reply.TextFormat = TextFormatTypes.Plain;
                    
                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>
                        {
                            new CardAction { Title = "Регистрация", Type=ActionTypes.ImBack, Value="/registration" },
                        }
                    };

                    await context.PostAsync(reply);
                }
                else
                {
                    var reply = context.MakeMessage();
                    reply.Text = $"Приветствую, {response.XMLPort.Employee.First().FirstName}";
                    reply.Type = ActivityTypes.Message;
                    reply.TextFormat = TextFormatTypes.Plain;

                    reply.SuggestedActions = new SuggestedActions
                    {
                        Actions = new List<CardAction>
                        {
                            new CardAction { Title = "Меню", Type=ActionTypes.ImBack, Value="/menu" },
                        }
                    };

                    await context.PostAsync(reply);
                }
            }
            else if (message.Text.ToLower().Equals("/registration", StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call(new RegistrationDialog(), RegistrationDialogResumeAfter);
            }
            else if (message.Text.ToLower().Equals("/menu", StringComparison.InvariantCultureIgnoreCase))
            {
                await context.PostAsync("Menu");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                await context.PostAsync("Ничего не понял");
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
        }
        
        private async Task RegistrationDialogResumeAfter(IDialogContext context, IAwaitable<Employee> result)
        {
            var employee = await result;
            if(employee.FirstName != null)
            {
                var reply = context.MakeMessage();
                reply.Text = $"Приветствую, {employee.FirstName}";
                reply.Type = ActivityTypes.Message;
                reply.TextFormat = TextFormatTypes.Plain;

                reply.SuggestedActions = new SuggestedActions
                {
                    Actions = new List<CardAction>
                    {
                        new CardAction { Title = "Меню", Type=ActionTypes.ImBack, Value="/menu" },
                    }
                };
            }
            else
            {
                var reply = context.MakeMessage();
                reply.Text = "Вам необходимо зарегистрироваться";
                reply.Type = ActivityTypes.Message;
                reply.TextFormat = TextFormatTypes.Plain;

                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>
                    {
                        new CardAction { Title = "Регистрация", Type=ActionTypes.ImBack, Value="/registration" },
                    }
                };

                await context.PostAsync(reply);
            }
        }
    }
}
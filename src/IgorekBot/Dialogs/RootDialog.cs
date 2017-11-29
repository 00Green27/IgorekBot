﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Resource;
using Microsoft.Bot.Connector;
using NMSService.NMSServiceReference;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            var message = await result;

            await SendWelcomeMessageAsync(context);

//            var message = await result;
//
//            if (message.Text == RegistrationCommand)
//            {
//                await this.SendRegistrationAsync(context);
//            }
//            else
//            {
//                var helloMessage = context.MakeMessage();
//                helloMessage.InputHint = InputHints.AcceptingInput;
//
//                helloMessage.Attachments = new List<Attachment>
//                {
//                    new HeroCard("Привет, я бот!")
//                    {
//                        Buttons = new List<CardAction>
//                        {
//                            new CardAction(ActionTypes.ImBack, "Регистрация", value: RegistrationCommand)
//                        }
//                    }.ToAttachment()
//                };
//
//                await context.PostAsync(helloMessage);
//            }
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            var helloMessage = context.MakeMessage();
            helloMessage.Text = "Привет, я бот!";
            //var reply = activity.CreateReply("I have colors in mind, but need your help to choose the best one.");
            helloMessage.Type = ActivityTypes.Message;
            helloMessage.TextFormat = TextFormatTypes.Plain;

            helloMessage.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title = "Регистрация", Type=ActionTypes.ImBack, Value="/registration" },
                }
            };

            await context.PostAsync(helloMessage);

            context.Call(new AuthenticationDialog(), AuthenticationDialogResumeAfter);
        }
        
        private async Task AuthenticationDialogResumeAfter(IDialogContext context, IAwaitable<Employee> result)
        {
            var employee = await result;

            await context.PostAsync($"Приветствую, {employee.FirstName}");
        }
    }
}
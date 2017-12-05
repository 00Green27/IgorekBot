using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using IgorekBot.BLL.Interfaces;
using IgorekBot.Properties;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using IgorekBot.BLL.Models;
using NMSService.NMSServiceReference;
using System.Linq;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class TimeSheetDialog : IDialog<object>
    {
        private readonly ITimeSheetService _service;
        private Employee _employee;

        public TimeSheetDialog(ITimeSheetService service)
        {
            SetField.NotNull(out _service, nameof(service), service);
        }
        
        public async Task StartAsync(IDialogContext context)
        {
            var activity = context.Activity;
            var reply = CreateMenu(context);
            await context.PostAsync(reply.AsMessageActivity());

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.Equals(Resources.MenuCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Done(new object());
            }
            else if (message.Text.Equals(Resources.ProjectsCommand, StringComparison.InvariantCultureIgnoreCase))
            {

                ShowProjects(context, message);
            }
            else
            {
                await context.PostAsync(Resources.TimeSheetDialog_Didnt_Understand_Message);
            }
        }

        private void ShowProjects(IDialogContext context, IMessageActivity message)
        {
            if (!context.UserData.TryGetValue(@"profile", out _employee))
            {
                var response = _service.GetUserById(new GetUserByIdRequest { ChannelId = message?.From?.Id });
                _employee = response.XMLPort.Employee.FirstOrDefault();
            }

            var res = _service.GetUserProjects(new GetUserProjectsRequest { UserId = _employee.No });
            PromptDialog.Choice(context, OnProjectSelected, res.XMLPort.Projects.Select(p => p.ProjectNo), "Выберите проект?");

        }

        private async Task OnProjectSelected(IDialogContext context, IAwaitable<string> result)
        {
            var projectId = await result;
            var res = _service.GetProjectTasks(new GetProjectTasksRequest { UserId = _employee.No, ProjectId = projectId });

            var reply = context.MakeMessage();

            foreach (var item in res.XMLPort.Projects)
            {

            }

            var heroCard = new HeroCard
            {
                Title = "dd",
                Buttons = res.XMLPort.Projects.Select(p => new CardAction(ActionTypes.PostBack, p.TaskDescription, value: p.TaskNo)).ToList()

            };

            reply.Attachments.Add(heroCard.ToAttachment());

            await context.PostAsync(reply);
            PromptDialog.Choice(context, OnProjectSelected, res.XMLPort.Projects.Select(p => p.TaskNo), "Выберите задачу?");
        }

        public static IMessageActivity CreateMenu(IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.Text = Resources.TimeSheetDialog_Main_Message;

            reply.SuggestedActions = new SuggestedActions
            {
                Actions = new List<CardAction>
                    {
                        new CardAction { Title = Resources.MenuCommand, Type=ActionTypes.PostBack, Value = Resources.MenuCommand },
                        new CardAction { Title = Resources.HoursCommand, Type=ActionTypes.PostBack, Value = Resources.HoursCommand  },
                        new CardAction { Title = Resources.ProjectsCommand, Type=ActionTypes.PostBack, Value = Resources.ProjectsCommand },
                        new CardAction { Title = Resources.NotificationsCommand, Type=ActionTypes.PostBack, Value = Resources.NotificationsCommand },
                    }
            };

            return reply;
        }
    }
}
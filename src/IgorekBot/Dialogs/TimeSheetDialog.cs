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
using IgorekBot.Helpers;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class TimeSheetDialog : IDialog<object>
    {
        private readonly ITimeSheetService _service;
        private Employee _employee;
        private List<Projects> _projects;
        private List<Projects1> _tasks;

        public TimeSheetDialog(ITimeSheetService service)
        {
            SetField.NotNull(out _service, nameof(service), service);
        }
        
        public async Task StartAsync(IDialogContext context)
        {
            var activity = context.Activity;
            var reply = MenuHelper.CreateMenu(context, new List<string> { Resources.MenuCommand, Resources.HoursCommand, Resources.ProjectsCommand }, Resources.NotificationsCommand);
            await context.PostAsync(reply);

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
                context.UserData.SetValue(@"profile", _employee);            
            }

            var res = _service.GetUserProjects(new GetUserProjectsRequest { UserId = _employee.No });
            _projects = res.XMLPort.Projects.ToList();
            PromptDialog.Choice(context, OnProjectSelected, _projects.Select(p => p.ProjectDescription), "Выберите проект?");
        }

        private async Task OnProjectSelected(IDialogContext context, IAwaitable<string> result)
        {
            var project = await result;

            var res = _service.GetProjectTasks(new GetProjectTasksRequest { UserId = _employee.No, ProjectId = _projects.First(p => p.ProjectDescription == project).ProjectNo });

            //var reply = context.MakeMessage();

            //await context.PostAsync(reply);
            _tasks = res.XMLPort.Projects.ToList();
            PromptDialog.Choice(context, OnTaskSelected, _tasks.Select(p => p.TaskDescription), "Выберите задачу?");
        }

        private async Task OnTaskSelected(IDialogContext context, IAwaitable<string> result)
        {
            var task = await result;
            await context.PostAsync(task);
            context.Wait(MessageReceivedAsync);
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
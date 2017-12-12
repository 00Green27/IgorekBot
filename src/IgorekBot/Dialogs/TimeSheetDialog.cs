using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using IgorekBot.Properties;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using IgorekBot.BLL.Models;
using NMSService.NMSServiceReference;
using System.Linq;
using IgorekBot.BLL.Services;
using IgorekBot.Data.Models;
using IgorekBot.Helpers;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class TimeSheetDialog : IDialog<object>
    {
        private readonly IBotService _botSvc;
        private readonly ITimeSheetService _timeSheetSvc;

        private readonly IEnumerable<string> _mainMenu = new List<string>
        {
            Resources.MenuCommand,
            Resources.HoursCommand,
            Resources.ProjectsCommand,
            Resources.NotificationsCommand
        };

        private UserProfile _profile;
        private IEnumerable<Projects> _projects;
        private IEnumerable<ProjectTask> _tasks;


        public TimeSheetDialog(IBotService botSvc, ITimeSheetService timeSheetSvc)
        {
            SetField.NotNull(out _botSvc, nameof(botSvc), botSvc);
            SetField.NotNull(out _timeSheetSvc, nameof(timeSheetSvc), timeSheetSvc);
        }


        public async Task StartAsync(IDialogContext context)
        {
            var reply = MenuHelper.CreateMenu(context, _mainMenu, Resources.TimeSheetDialog_Main_Message);
            await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (!context.UserData.TryGetValue(@"profile", out _profile))
            {
                _profile = await _botSvc.GetUserProfileByUserId(message.From.Id);
                context.UserData.SetValue(@"profile", _profile);
            }

            if (message.Text.Equals(Resources.MenuCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Done<object>(null);
            }
            else if (message.Text.Equals(Resources.ProjectsCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                await context.PostAsync(MenuHelper.CreateMenu(context, new List<string> {Resources.MenuCommand},
                    "Получаю список проектов..."));

                //context.Call(new ProjectsDialog(_timeSheetSvc), OnProjectSelected);
                //PromptDialog.Choice(context, OnProjectSelected, _projects.Select(p => p.ProjectNo), Resources.TimeSheetDialog_Project_Choice_Message, descriptions: _projects.Select(p => p.ProjectDescription));

                var response =
                    _timeSheetSvc.GetUserProjects(new GetUserProjectsRequest {UserId = _profile.EmployeeCode});
                _projects = response.Projects.ToList();
                var reply = CreateMessageWithHeroCard(context,
                    _projects.Select(p => new CardAction {Title = p.ProjectDescription, Value = p.ProjectNo}),
                    Resources.TimeSheetDialog_Project_Choice_Message);
                await context.PostAsync(reply);
                context.Wait<IMessageActivity>(OnProjectSelected);
            }
            else
            {
                await context.PostAsync(Resources.TimeSheetDialog_Didnt_Understand_Message);
            }
        }

        private async Task OnProjectSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.Equals(Resources.MenuCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Done<object>(null);
            }
            else
            {
                var response = _timeSheetSvc.GetProjectTasks(new GetProjectTasksRequest
                {
                    UserId = _profile.EmployeeCode,
                    ProjectId = _projects.First(p => p.ProjectNo == message.Text).ProjectNo
                });
                _tasks = response.ProjectTasks.ToList();
                var reply = CreateMessageWithHeroCard(context,
                    _tasks.Select(p => new CardAction {Title = p.TaskDescription, Value = p.TaskNo}),
                    Resources.TimeSheetDialog_Task_Choice_Message);
                await context.PostAsync(reply);
                context.Wait<IMessageActivity>(OnTaskSelected);
            }
        }

        private async Task OnTaskSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.Equals(Resources.MenuCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Done<object>(null);
            }

            ShowOptions(context);
        }

        private void ShowOptions(IDialogContext context)
        {

            var actions = new List<CardAction>
            {
                new CardAction {Title = Resources.TimeSheetDialog_WriteOff_Action, Value = Resources.TimeSheetDialog_WriteOff_Action},
                new CardAction {Title = Resources.TimeSheetDialog_Add_To_StopList_Action, Value = Resources.TimeSheetDialog_Add_To_StopList_Action}
            };

            var reply = CreateMessageWithHeroCard(context, actions, Resources.TimeSheetDialog_Main_Message);
            context.PostAsync(reply);
            context.Wait<IMessageActivity>(OnOptionSelected);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.Equals(Resources.TimeSheetDialog_WriteOff_Action,
                StringComparison.InvariantCultureIgnoreCase))
            {
                
            } else if (message.Text.Equals(Resources.TimeSheetDialog_Add_To_StopList_Action,
                StringComparison.InvariantCultureIgnoreCase))
            {
                
            } else if (message.Text.Equals(Resources.MenuCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Done<object>(null);
            }
        }

        //        private async Task OnProjectSelected(IDialogContext context, IAwaitable<string> result)
        //        {
        //            var projectNo = await result;
        //
        //            PromptDialog.Choice(context, OnTaskSelected, _tasks.Select(p => p.TaskNo), Resources.TimeSheetDialog_Task_Choice_Message, descriptions: _tasks.Select(p => p.TaskDescription));
        //        }


        //        private async Task OnProjectSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        //        {
        //            var message = await result;
        //            var response = _timeSheetSvc.GetProjectTasks(new GetProjectTasksRequest { UserId = _profile.EmployeeCode, ProjectId = _projects.First(p => p.ProjectNo == message.Text).ProjectNo });
        //            _tasks = response.ProjectTasks.ToList();
        //
        //            var reply = CreateMessageWithHeroCard(context, _tasks.Select(p => new CardAction { Title = p.TaskDescription, Value = p.TaskNo }));
        //            await context.PostAsync(reply);
        //            context.Wait<IMessageActivity>(OnTaskSelected);
        //
        //            //PromptDialog.Choice(context, OnTaskSelected, _tasks.Select(p => p.TaskNo), Resources.TimeSheetDialog_Task_Choice_Message, descriptions: _tasks.Select(p => p.TaskDescription));
        //        }
        //
        //        private async Task OnTaskSelected(IDialogContext context, IAwaitable<string> result)
        //        {
        //            var taskNo = await result;
        //            context.Wait(MessageReceivedAsync);
        //        }

        private IMessageActivity CreateMessageWithHeroCard(IDialogContext context, IEnumerable<CardAction> actions,
            string text)
        {
            var reply = context
                .MakeMessage(); //MenuHelper.CreateMenu(context, new List<string> { Resources.MenuCommand });


            HeroCard projectsCard = new HeroCard
            {
                Text = text,
                Buttons = actions.ToList()
            };
            reply.Attachments = new List<Attachment>();
            reply.Attachments.Add(projectsCard.ToAttachment());

            return reply;
        }
    }
}
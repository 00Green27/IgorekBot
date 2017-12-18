using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Data.Models;
using IgorekBot.Helpers;
using IgorekBot.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using NMSService.NMSServiceReference;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class TimeSheetDialog : IDialog<object>
    {
        private readonly IBotService _botSvc;
        private readonly ITimeSheetService _timeSheetSvc;
        private UserProfile _profile;
        private IEnumerable<Projects> _projects;
        private IEnumerable<ProjectTask> _tasks;
        private string _taskNo;
        private readonly IEnumerable<string> _mainMenu = new List<string>
                    {
                        Resources.BackCommand,
                        Resources.HoursCommand,
                        Resources.ProjectsCommand,
                        Resources.NotificationsCommand,
                        Resources.StoplistCommand
                    };

        //private readonly CardAction[][] _mainMenu = new CardAction[3][]
        //{
        //    new[] {
        //        new CardAction {Title =  Resources.BackCommand, Value = Resources.BackCommand
        //        }
        //    },
        //    new[] {
        //        new CardAction {Title =  Resources.HoursCommand, Value = Resources.HoursCommand},
        //        new CardAction {Title =  Resources.ProjectsCommand, Value = Resources.ProjectsCommand}
        //    },
        //    new[] {
        //        new CardAction {Title =  Resources.NotificationsCommand, Value = Resources.NotificationsCommand},
        //        new CardAction {Title =  Resources.StoplistCommand, Value = Resources.StoplistCommand}
        //    }
        //};


        private string _projectNo;


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

            if (message.Text.Equals(Resources.BackCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Done<object>(null);
            }
            else if (message.Text.Equals(Resources.HoursCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call(new TimeSheetStatisticDialog(_botSvc, _timeSheetSvc), AfterResume);
            }
            else if (message.Text.Equals(Resources.ProjectsCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                await context.PostAsync(MenuHelper.CreateMenu(context, new List<string> {Resources.BackCommand},
                    "Получаю список проектов..."));

                var response =
                    _timeSheetSvc.GetUserProjects(new GetUserProjectsRequest {UserId = _profile.EmployeeNo});
                _projects = response.Projects.ToList();

                CancelablePromptChoice<string>.Choice(context, AfterProjectSelected, _projects.Select(p => p.ProjectNo),
                    Resources.TimeSheetDialog_Project_Choice_Message,
                    descriptions: _projects.Select(p => p.ProjectDescription));
            }
            else if (message.Text.Equals(Resources.StoplistCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call(new StoplistDialog(_botSvc, _timeSheetSvc), AfterResume);
            }
            else
            {
                await context.PostAsync(Resources.TimeSheetDialog_Didnt_Understand_Message);
            }
        }

        private async Task AfterProjectSelected(IDialogContext context, IAwaitable<string> result)

        {
            _projectNo = await result;

            if (_projectNo == null)
            {
                await StartAsync(context);
            }
            else
            {
                var response = _timeSheetSvc.GetProjectTasks(new GetProjectTasksRequest
                {
                    UserId = _profile.EmployeeNo,
                    ProjectId = _projects.First(p => p.ProjectNo == _projectNo).ProjectNo
                });

                var hiddenTask = _botSvc.GetUserHiddenTasks(_profile);

                _tasks = response.ProjectTasks.Where(i => hiddenTask.All(e => e.ProjectNo != _projectNo && i.TaskNo != e.TaskNo)).ToList();
                CancelablePromptChoice<string>.Choice(context, AfterTaskSelected, _tasks.Select(t => t.TaskNo.Replace(".", "💩")),
                    Resources.TimeSheetDialog_Task_Choice_Message,
                    descriptions: _tasks.Select(p => p.TaskDescription));
            }
        }

        private async Task AfterTaskSelected(IDialogContext context, IAwaitable<string> result)
        {
            _taskNo = await result;

            if (_taskNo == null)
            {
                await StartAsync(context);
            }
            else
            {
                _taskNo = _taskNo.Replace("💩", ".");
                var list = new List<string>
                {
                    Resources.TimeSheetDialog_WriteOff_Action,
                    Resources.TimeSheetDialog_Add_To_StopList_Action
                };

                CancelablePromptChoice<string>.Choice(context, AfterTaskActionSelected, list,
                    Resources.TimeSheetDialog_Main_Message);
            }
        }

        private async Task AfterResume(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var retValue = await result;
            }
            catch (Exception e)
            {
                await context.PostAsync(e.Message);
            }
            await StartAsync(context);
        }

//        private async Task OnProjectSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
//        {
//            var message = await result;
//
//            if (message.Text.Equals(Resources.BackCommand, StringComparison.InvariantCultureIgnoreCase))
//            {
//                context.Done<object>(null);
//            }
//            else
//            {
//                var response = _timeSheetSvc.GetProjectTasks(new GetProjectTasksRequest
//                {
//                    UserId = _profile.EmployeeNo,
//                    ProjectId = _projects.First(p => p.ProjectNo == message.Text).ProjectNo
//                });
//                _tasks = response.ProjectTasks.ToList();
//                var reply = CreateMessageWithHeroCard(context,
//                    _tasks.Select(p => new CardAction {Title = p.TaskDescription, Value = p.TaskNo}),
//                    Resources.TimeSheetDialog_Task_Choice_Message);
//                await context.PostAsync(reply);
//                context.Wait<IMessageActivity>(OnTaskSelected);
//            }
//        }

        //        private async Task OnTaskSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        //        {
        //            var message = await result;
        //
        //            if (message.Text.Equals(Resources.BackCommand, StringComparison.InvariantCultureIgnoreCase))
        //                context.Done<object>(null);
        //
        //            ShowOptions(context);
        //        }

        //        private void ShowOptions(IDialogContext context)
        //        {
        //            var actions = new List<CardAction>
        //            {
        //                new CardAction
        //                {
        //                    Title = Resources.TimeSheetDialog_WriteOff_Action,
        //                    Value = Resources.TimeSheetDialog_WriteOff_Action
        //                },
        //                new CardAction
        //                {
        //                    Title = Resources.TimeSheetDialog_Add_To_StopList_Action,
        //                    Value = Resources.TimeSheetDialog_Add_To_StopList_Action
        //                }
        //            };
        //
        //            var reply = CreateMessageWithHeroCard(context, actions, Resources.TimeSheetDialog_Main_Message);
        //            context.PostAsync(reply);
        //            context.Wait<IMessageActivity>(OnOptionSelected);
        //        }

        //        private async Task OnOptionSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        //        {
        //            var message = await result;
        //
        //            if (message.Text.Equals(Resources.BackCommand, StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                context.Done<object>(null);
        //            }
        //            else if (message.Text.Equals(Resources.TimeSheetDialog_WriteOff_Action,
        //                StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                context.Call(new AddTimeSheetDialog(_botSvc, _timeSheetSvc), ResumeAfterTimeSheetAdded);
        //            }
        //            else if (message.Text.Equals(Resources.TimeSheetDialog_Add_To_StopList_Action,
        //                StringComparison.InvariantCultureIgnoreCase))
        //            {
        //            }
        //        }

        private Task ResumeAfterTimeSheetAdded(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

//        private async Task OnProjectSelected(IDialogContext context, IAwaitable<string> result)
//        {
//            var text = await result;
//
//
//            if (text.Equals(Resources.BackCommand, StringComparison.InvariantCultureIgnoreCase))
//            {
//                context.Done<object>(null);
//            }
//            else
//            {
//                var response = _timeSheetSvc.GetProjectTasks(new GetProjectTasksRequest
//                {
//                    UserId = _profile.EmployeeNo,
//                    ProjectId = _projects.First(p => p.ProjectNo == text).ProjectNo
//                });
//                _tasks = response.ProjectTasks.ToList();
//                PromptDialog.Choice(context, OnTaskSelected, _tasks.Select(p => p.TaskNo),
//                    Resources.TimeSheetDialog_Task_Choice_Message, descriptions: _tasks.Select(p => p.TaskDescription));
//            }
//        }
//
//        private async Task OnTaskSelected(IDialogContext context, IAwaitable<string> result)
//        {
//            var text = await result;
//
//            if (text.Equals(Resources.BackCommand, StringComparison.InvariantCultureIgnoreCase))
//            {
//                context.Done<object>(null);
//            }
//            else
//            {
//                var list = new List<string>
//                {
//                    Resources.TimeSheetDialog_WriteOff_Action,
//                    Resources.TimeSheetDialog_Add_To_StopList_Action
//                };
//                PromptDialog.Choice(context, AfterTaskActionSelected, list,
//                    Resources.TimeSheetDialog_Main_Message,
//                    descriptions: list);
//            }
////                ShowOptions(context);
//        }

        private async Task AfterTaskActionSelected(IDialogContext context, IAwaitable<string> result)
        {
            var action = await result;
            if (action == null)
            {
                await StartAsync(context);
            }
            else if (action.Equals(Resources.TimeSheetDialog_WriteOff_Action,
                StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call(new AddTimeSheetDialog(_botSvc, _timeSheetSvc, _tasks.First(t => t.TaskNo == _taskNo)), AfterTimeSheetAdded);
            }
            else if (action.Equals(Resources.TimeSheetDialog_Add_To_StopList_Action,
                StringComparison.InvariantCultureIgnoreCase))
            {
                await _botSvc.HideTask(new HiddenTask(_profile, _projectNo, _taskNo));
                await context.PostAsync($"Задача **{_tasks.First(t => t.TaskNo == _taskNo).TaskDescription}** добавлена в стоп-лист.");
                await StartAsync(context);
            }
        }

        private async Task AfterTimeSheetAdded(IDialogContext context, IAwaitable<object> result)
        {
            await StartAsync(context);
        }


        //        private async Task OnProjectSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        //        {
        //            var message = await result;
        //            var response = _timeSheetSvc.GetProjectTasks(new GetProjectTasksRequest { UserId = _profile.EmployeeNo, ProjectId = _projects.First(p => p.ProjectNo == message.Text).ProjectNo });
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


            var projectsCard = new HeroCard
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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private readonly IEnumerable<string> _mainMenu = new List<string>
        {
            Resources.BackCommand,
            Resources.HoursCommand,
            Resources.ProjectsCommand,
            Resources.NotificationsCommand,
            Resources.StoplistCommand
        };

        private readonly ITimeSheetService _timeSheetSvc;
        private UserProfile _profile;

//        private readonly KeyboardButton[][] _mainMenu = new KeyboardButton[3][]
//        {
//            new[] {
//                new KeyboardButton {Text =  Resources.BackCommand, Value = Resources.BackCommand
//                }
//            },
//            new[] {
//                new KeyboardButton {Text =  Resources.HoursCommand, Value = Resources.HoursCommand},
//                new KeyboardButton {Text =  Resources.ProjectsCommand, Value = Resources.ProjectsCommand}
//            },
//            new[] {
//                new KeyboardButton {Text =  Resources.NotificationsCommand, Value = Resources.NotificationsCommand},
//                new KeyboardButton {Text =  Resources.StoplistCommand, Value = Resources.StoplistCommand}
//            }
//        };


        private string _projectNo;
        private IEnumerable<Projects> _projects;
        private string _taskNo;
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

                _projects = GetProjects();

                CancelablePromptChoice<string>.Choice(context, AfterProjectSelected, _projects.Select(p => p.ProjectNo),
                    Resources.TimeSheetDialog_Project_Choice_Message,
                    descriptions: _projects.Select(p => p.ProjectDescription));
            }
            else if (message.Text.Equals(Resources.StoplistCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call(new StoplistDialog(_botSvc, _timeSheetSvc), AfterResume);
            }
            else if (message.Text.Equals(Resources.NotificationsCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call(new NotificationsDialog(_botSvc, _timeSheetSvc), AfterResume);
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

                _tasks = response.ProjectTasks.Where(t =>
                    hiddenTask.All(e => t.TaskNo != e.TaskNo) && !string.IsNullOrEmpty(t.AssignmentCode)).ToList();
                CancelablePromptChoice<string>.Choice(context, AfterTaskSelected,
                    _tasks.Select(t => t.TaskNo.Replace(".", "💩")),
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
        

        private Task ResumeAfterTimeSheetAdded(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        private async Task AfterTaskActionSelected(IDialogContext context, IAwaitable<string> result)
        {
            var action = await result;
            if (action == null)
            {
                await StartAsync(context);
            }
            else
            {
                var task = _tasks.First(t => t.TaskNo == _taskNo);
                task.ProjectNo = _projectNo;
                if (action.Equals(Resources.TimeSheetDialog_WriteOff_Action,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Call(new AddTimeSheetDialog(_botSvc, _timeSheetSvc, task), AfterTimeSheetAdded);
                }
                else if (action.Equals(Resources.TimeSheetDialog_Add_To_StopList_Action,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    await _botSvc.HideTask(new HiddenTask(_profile, _projectNo, _taskNo));
                    await context.PostAsync($"Задача **{task.TaskDescription}** добавлена в стоп-лист.");
                    await StartAsync(context);
                }
            }
        }

        private async Task AfterTimeSheetAdded(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                await StartAsync(context);
            }
            catch (Exception)
            {
                await context.PostAsync("Что-то пошло не так");
            }
        }

        private IMessageActivity CreateMessageWithHeroCard(IDialogContext context, IEnumerable<CardAction> actions, string text)
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

        private List<Projects> GetProjects()
        {
            var response = _timeSheetSvc.GetUserProjects(new GetUserProjectsRequest {UserId = _profile.EmployeeNo});

            var projects = response.Projects.ToList();

            var list = new ConcurrentBag<Projects>();

            Parallel.ForEach(projects, p =>
            {
                var t = GetTasks(p.ProjectNo);
                if (t.Any())
                    list.Add(p);
            });
            return list.ToList();
        }

        private List<ProjectTask> GetTasks(string projectNo)
        {
            var response = _timeSheetSvc.GetProjectTasks(new GetProjectTasksRequest
            {
                UserId = _profile.EmployeeNo,
                ProjectId = projectNo
            });

            var hiddenTask = _botSvc.GetUserHiddenTasks(_profile);

            return response.ProjectTasks.Where(t =>
                hiddenTask.All(e => t.TaskNo != e.TaskNo) && !string.IsNullOrEmpty(t.AssignmentCode)).ToList();
        }
    }
}
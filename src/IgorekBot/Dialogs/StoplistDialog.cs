using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Data.Models;
using IgorekBot.Helpers;
using IgorekBot.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using NMSService.NMSServiceReference;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class StoplistDialog : IDialog<object>
    {
        private readonly IBotService _botSvc;
        private readonly ITimeSheetService _timeSheetSvc;
        private UserProfile _profile;
        private List<Projects> _projects;
        private List<ProjectTask> _tasks;
        private string _taskNo;

        public StoplistDialog(IBotService botSvc, ITimeSheetService timeSheetSvc)
        {
            SetField.NotNull(out _botSvc, nameof(botSvc), botSvc);
            SetField.NotNull(out _timeSheetSvc, nameof(timeSheetSvc), timeSheetSvc);
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.UserData.TryGetValue(@"profile", out _profile);

            await context.PostAsync(MenuHelper.CreateMenu(context, new List<string> {Resources.BackCommand},
                "Получаю список проектов..."));

            var response =
                _timeSheetSvc.GetUserProjects(new GetUserProjectsRequest {UserId = _profile.EmployeeNo});
            _projects = response.Projects.ToList();

            CancelablePromptChoice<string>.Choice(context, AfterProjectSelected, _projects.Select(p => p.ProjectNo),
                Resources.TimeSheetDialog_Project_Choice_Message,
                descriptions: _projects.Select(p => p.ProjectDescription));
        }

        private async Task AfterProjectSelected(IDialogContext context, IAwaitable<string> result)
        {
            var projectNo = await result;

            if (projectNo == null)
            {
                context.Done(false);
            }
            else
            {
                var response = _timeSheetSvc.GetProjectTasks(new GetProjectTasksRequest
                {
                    UserId = _profile.EmployeeNo,
                    ProjectId = _projects.First(p => p.ProjectNo == projectNo).ProjectNo
                });

                var hiddenTask = _botSvc.GetUserHiddenTask(_profile);

                _tasks = response.ProjectTasks.Join(hiddenTask, t => t.TaskNo, ht => ht.TaskNo, (t, ht) => t).ToList();
                if (_tasks.Count != 0)
                {
                    CancelablePromptChoice<string>.Choice(context, AfterTaskSelected,
                        _tasks.Select(t => t.TaskNo.Replace(".", "💩")),
                        Resources.TimeSheetDialog_Task_Choice_Message,
                        descriptions: _tasks.Select(p => p.TaskDescription));
                }
                else
                {
                    await context.PostAsync("По данному проекту нет задач в стоп-листе");
                    context.Done(true);
                }
            }
        }

        private async Task AfterTaskSelected(IDialogContext context, IAwaitable<string> result)
        {
            _taskNo = await result;

            if (_taskNo == null)
            {
                context.Done(false);
            }
            else
            {
                _taskNo = _taskNo.Replace("💩", ".");
                var list = new List<string>
                {
                    Resources.TimeSheetDialog_Remove_To_StopList_Action
                };

                CancelablePromptChoice<string>.Choice(context, AfterTaskActionSelected, list,
                    Resources.TimeSheetDialog_Main_Message);
            }
        }

        private async Task AfterTaskActionSelected(IDialogContext context, IAwaitable<string> result)
        {
            var action = await result;
            if (action == null)
            {
                context.Done(false);
            }
            else if (action.Equals(Resources.TimeSheetDialog_Remove_To_StopList_Action,
                StringComparison.InvariantCultureIgnoreCase))
            {
                await _botSvc.ShowTask(new HiddenTask(_profile, _taskNo));
                await context.PostAsync(
                    $"Задача **{_tasks.First(t => t.TaskNo == _taskNo).TaskDescription}** удалена из стоп-листа.");
                context.Done(true);
            }
        }
    }
}
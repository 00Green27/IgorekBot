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

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class AddTimeSheetDialog : IDialog<object>
    {
        private readonly IBotService _botSvc;
        private readonly ProjectTask _task;
        private readonly ITimeSheetService _timeSheetSvc;
        private string _comment;
        private DateTime _date;
        private int _hours;
        private UserProfile _profile;
        private IEnumerable<Workday> _workdays;


        public AddTimeSheetDialog(IBotService botSvc, ITimeSheetService timeSheetSvc, ProjectTask task)
        {
            SetField.NotNull(out _botSvc, nameof(botSvc), botSvc);
            SetField.NotNull(out _timeSheetSvc, nameof(timeSheetSvc), timeSheetSvc);
            SetField.NotNull(out _task, nameof(task), task);
            SetField.NotNull(out _task, nameof(task), task);
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.UserData.TryGetValue(@"profile", out _profile);
            await DaysButtons(context, Resources.CurrentWeekCommand, 1);
        }

        private async Task AfterWorkdaySelected(IDialogContext context, IAwaitable<string> result)
        {
            var text = await result;
            if (text.Equals(Resources.CurrentWeekCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                await DaysButtons(context, Resources.PrevWeekCommand);
            }
            else if (text.Equals(Resources.PrevWeekCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                await DaysButtons(context, Resources.CurrentWeekCommand, 1);
            }
            else
            {
                var workday = _workdays.First(d => d.ToString() == text);
                _date = workday.Date;

                var h = 1;
                if (workday.WorkHours < 8)
                {
                    h = 8 - (int) workday.WorkHours;
                }

                CancelablePromptChoice<int>.Choice(context, AfterHoursEntered,
                    Enumerable.Range(1, h), "Количество часов");
            }
        }

        private async Task AfterHoursEntered(IDialogContext context, IAwaitable<int> result)
        {
            _hours = await result;
            var prompt = new PromptDialog.PromptString("Коментарий к задаче", null, 3);
            context.Call(prompt, AfterCommenEntered);
        }

        private async Task AfterCommenEntered(IDialogContext context, IAwaitable<string> result)
        {
            _comment = await result;
            var response = _timeSheetSvc.AddTimeSheet(new AddTimeSheetRequest
            {
                AssignmentCode = _task.AssignmentCode,
                Comment = _comment,
                Date = _date,
                EmployeeNo = _profile.EmployeeNo,
                Hours = _hours,
                ProjectNo = _task.ProjectNo
            });

            var message = context.MakeMessage();
            if (response.Result == 1)
                message.Text = response.ErrorText;
            else
                message.Text = "**ТШ создан**";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        private async Task DaysButtons(IDialogContext context, string lastButton, int weekAgo = 0)
        {
            var startOfWeek = DateTime.Now.StartOfWeek(weekAgo);
            var endOfWeek = startOfWeek.AddDays(4);

            await context.PostAsync($"Неделя [{startOfWeek:dd.MM.yyyy} - {endOfWeek:dd.MM.yyyy}]");

            var response = _timeSheetSvc.GetWorkdays(new GetTimeSheetsPerWeekRequest
            {
                EmployeeNo = _profile.EmployeeNo,
                StartDate = startOfWeek,
                EndDate = endOfWeek
            });

            if (response.Result == 0)
            {
                _workdays = response.Workdays;
                var days = _workdays.Select(d => d.ToString()).ToList();
                days.Add(lastButton);
                CancelablePromptChoice<string>.Choice(context, AfterWorkdaySelected, days, "Выберите дату списания");
            }
            else
            {
                context.Fail(new Exception(response.ErrorText));
            }
        }
    }
}
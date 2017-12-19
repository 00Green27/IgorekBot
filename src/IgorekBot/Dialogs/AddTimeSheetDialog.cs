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
using Microsoft.Bot.Connector;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class AddTimeSheetDialog : IDialog<object>
    {
        private readonly IBotService _botSvc;
        private readonly ITimeSheetService _timeSheetSvc;
        private UserProfile _profile;
        private readonly List<string> _mainMenu = new List<string> { Resources.BackCommand, Resources.PrevWeekCommand, Resources.CurrentWeekCommand, Resources.NextWeekCommand };
        private IEnumerable<Workday> _workdays;
        private DateTime _date;
        public int _hours;
        private string _comment;
        private ProjectTask _task;


        public AddTimeSheetDialog(IBotService botSvc, ITimeSheetService timeSheetSvc, ProjectTask task)
        {
            SetField.NotNull(out _botSvc, nameof(botSvc), botSvc);
            SetField.NotNull(out _timeSheetSvc, nameof(timeSheetSvc), timeSheetSvc);
            SetField.NotNull(out _task, nameof(task), task);

        }

        public async Task StartAsync(IDialogContext context)
        {
            context.UserData.TryGetValue(@"profile", out _profile);
            await DaysButtons(context, "Текущая неделя", 1);
        }

        private async Task AfterWorkdaySelected(IDialogContext context, IAwaitable<string> result)
        {
            var text = await result;
            if (text.Equals("Текущая неделя", StringComparison.InvariantCultureIgnoreCase))
            {
                await DaysButtons(context, "Предыдущая неделя");
            }
            else if(text.Equals("Предыдущая неделя", StringComparison.InvariantCultureIgnoreCase))
            {
                await DaysButtons(context, "Текущая неделя", 1);
            }
            else
            {
                var workday = _workdays.First(d => d.ToString() == text);
                _date = workday.Date;
                
                CancelablePromptChoice<int>.Choice(context, AfterHoursEntered, Enumerable.Range(1, 8 - workday.WorkHours), "Количество часов");


                //                var promptOption = new PromptOptions<long>("Количество часов");
                //                var prompt = new PromptDialog.PromptInt64(promptOption, min: 1, max: 8);
                //var prompt = new PromptStringRegex("Количество часов", "[1-8]");

                //context.Call(prompt, AfterHoursEntered);
                //                var getTimeSheetsPerDayResponse = _timeSheetSvc.GetTimeSheetsPerDay(new GetTimeSheetsPerDayRequest
                //                {
                //                    EmployeeNo = _profile.EmployeeNo,
                //                    Date = _workdays.First(d => d.ToString() == text).Date
                //                });
            }

        }

        private async Task AfterHoursEntered(IDialogContext context, IAwaitable<int> result)
        {
            _hours = await result;
            var prompt = new PromptDialog.PromptString("Коментарий", null, 3);
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
                TaskNo = _task.TaskNo
            });

            if (response.Result == 1)
            {
                context.Fail(new Exception(response.ErrorText));
            }
            else
            {
                var message = context.MakeMessage();
                message.TextFormat = TextFormatTypes.Markdown;
                message.Text = "ТШ создан";
                await context.PostAsync(message);
                context.Done(true);
            }
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
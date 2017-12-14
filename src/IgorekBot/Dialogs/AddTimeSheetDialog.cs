using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Data.Models;
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
        public AddTimeSheetDialog(IBotService botSvc, ITimeSheetService timeSheetSvc)
        {
            SetField.NotNull(out _botSvc, nameof(botSvc), botSvc);
            SetField.NotNull(out _timeSheetSvc, nameof(timeSheetSvc), timeSheetSvc);
        }

        public Task StartAsync(IDialogContext context)
        {
            context.UserData.TryGetValue(@"profile", out _profile);
            var response = _timeSheetSvc.GetTimeSheetsPerWeek(new GetTimeSheetsPerWeekRequest { EmployeeNo = _profile.EmployeeCode, StartDate = new DateTime(DateTime.Today.Year, 1, 1) , EndDate = DateTime.Today});
            var response1 = _timeSheetSvc.GetTimeSheetsPerDay(new GetTimeSheetsPerDayRequest() { EmployeeNo = _profile.EmployeeCode, Date = new DateTime(2017,11,15)});

            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            throw new NotImplementedException();
        }
    }
}
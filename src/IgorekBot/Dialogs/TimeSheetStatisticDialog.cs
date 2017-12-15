using System;
using System.Collections.Generic;
using System.Globalization;
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
using NLog.Time;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class TimeSheetStatisticDialog : IDialog<object>
    {
        private readonly IBotService _botSvc;
        private readonly ITimeSheetService _timeSheetSvc;
        private UserProfile _profile;
        public TimeSheetStatisticDialog(IBotService botSvc, ITimeSheetService timeSheetSvc)
        {
            SetField.NotNull(out _botSvc, nameof(botSvc), botSvc);
            SetField.NotNull(out _timeSheetSvc, nameof(timeSheetSvc), timeSheetSvc);
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.UserData.TryGetValue(@"profile", out _profile);

            await context.PostAsync(MenuHelper.CreateMenu(context, new List<string> { Resources.MenuCommand },
                "Готовлю отчет..."));

            var message = context.MakeMessage();
            message.Attachments.Add(GenerateStatistics());
            await context.PostAsync(message);

            context.Wait(MessageReceivedAsync);
        }

        private Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            context.Done<object>(null);
            return Task.CompletedTask;
        }

        private Attachment GenerateStatistics()
        {
            var startOfWeek = DateTime.Now.StartOfWeek(1);
            var response = _timeSheetSvc.GetTimeSheetsPerWeek(new GetTimeSheetsPerWeekRequest { EmployeeNo = _profile.EmployeeCode, StartDate = startOfWeek, EndDate = startOfWeek.AddDays(6) });
            
            var facts = response.TimeSeets.AsEnumerable().Select(t => new Fact(DateTime.ParseExact(t.PostingDate[0], "MM/dd/yy", CultureInfo.InvariantCulture).ToString("dddd"), t.Quantity[0])).ToList();
            var writeOffHours = facts.Select(f => int.Parse(f.Value)).Sum().ToString();
            ReceiptCard receiptCard = new ReceiptCard
            {
                Title = $"С {startOfWeek.ToShortDateString()} по {startOfWeek.AddDays(6).ToShortDateString()} списано",
                Facts = facts,
                Total = $"{writeOffHours} из 40"
            };
            return receiptCard.ToAttachment();
        }
    }
}
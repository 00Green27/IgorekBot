using System;
using System.Collections.Generic;
using System.Globalization;
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
            await GetStatistics(context, Resources.CurrentWeekCommand, 1);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (message.Text.Equals(Resources.CurrentWeekCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                await GetStatistics(context, Resources.PrevWeekCommand);
            }
            else if (message.Text.Equals(Resources.PrevWeekCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                await GetStatistics(context, Resources.CurrentWeekCommand, 1);
            } 
            else
            {
                context.Done<object>(null);
            }
        }

        private async Task GetStatistics(IDialogContext context, string lastButton, int weekAgo = 0)
        {
            await context.PostAsync(MenuHelper.CreateMenu(context, new List<string> { Resources.BackCommand },
                "Готовлю отчет..."));

            var message = context.MakeMessage();
            var attachment = GenerateStatistics(lastButton, weekAgo);
            //if (attachment != null)
            //    message.Attachments.Add(attachment);
            //else
            //    message.Text = "За прошедшую неделю списано 0 часов.";
            message.Attachments.Add(attachment);
            await context.PostAsync(message);

            context.Wait(MessageReceivedAsync);
        }

        private Attachment GenerateStatistics(string lastButton, int weekAgo)
        {
            var startOfWeek = DateTime.Now.StartOfWeek(weekAgo);
            var response = _timeSheetSvc.GetWorkdays(new GetTimeSheetsPerWeekRequest
            {
                EmployeeNo = _profile.EmployeeNo,
                StartDate = startOfWeek,
                EndDate = startOfWeek.AddDays(4)
            });

            var ru = new CultureInfo("ru-RU");
            var facts = response.Workdays.Select(t =>
            {
                try
                {
                    return new Fact(t.Date.ToString("dddd", ru), t.WorkHours.ToString(CultureInfo.InvariantCulture));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return null;
            }).Where(f => f != null).ToList();
            var writeOffHours = facts.Select(f => int.Parse(f.Value)).Sum();
            //if (writeOffHours == 0)
            //    return null;

            var receiptCard = new ReceiptCard
            {
                Title = $"С {startOfWeek:dd.MM.yyyy} по {startOfWeek.AddDays(4):dd.MM.yyyy} списано",
                Facts = facts,
                Total = $"{writeOffHours} из 40",
                Buttons = new List<CardAction>
                {
                    new  CardAction(ActionTypes.PostBack, lastButton, value: lastButton)
                }
            };
            return receiptCard.ToAttachment();
        }
    }
}
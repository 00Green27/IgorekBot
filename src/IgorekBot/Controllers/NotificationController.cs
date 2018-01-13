using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace IgorekBot.Controllers
{
    public class NotificationController : ApiController
    {
        public static string MicrosoftAppId { get; set; }
            = ConfigurationManager.AppSettings["MicrosoftAppId"];

        public static string MicrosoftAppPassword { get; set; }
            = ConfigurationManager.AppSettings["MicrosoftAppPassword"];

        private readonly IBotService _botSvc;
        private readonly ITimeSheetService _timeSheetSvc;

        public NotificationController()
        {
            _botSvc = new BotService();
            _timeSheetSvc = new TimeSheetService();
        }

        [HttpGet]
        [Route("notify")]
        public async Task<HttpResponseMessage> Notify()
        {
            var conversationReferences = _botSvc.GetConversationReferences();
            foreach (var encodedRef in conversationReferences)
            {
                var convRef = UrlToken.Decode<ConversationReference>(encodedRef);
                var serviceUrl = new Uri(convRef.ServiceUrl);
                MicrosoftAppCredentials.TrustServiceUrl(convRef.ServiceUrl);

                var connector = new ConnectorClient(serviceUrl, MicrosoftAppId, MicrosoftAppPassword);
                var existingConversationMessage = convRef.GetPostToUserMessage();
                var writeOffHours = await GetWriteOffHoursAsync(convRef.User.Id);
                existingConversationMessage.Text = $"Не забудь заполнить таймшит за текущую неделю. Списано часов: {writeOffHours}";
                connector.Conversations.SendToConversation(existingConversationMessage);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        private async Task<int> GetWriteOffHoursAsync(string userId)
        {
            var profile = await _botSvc.GetUserProfileByUserId(userId);
            int weekAgo = DateTime.Today.DayOfWeek > DayOfWeek.Friday ? 0 : 1;
            var startOfWeek = DateTime.Now.StartOfWeek(weekAgo);
            var response = _timeSheetSvc.GetWorkdays(new GetTimeSheetsPerWeekRequest
            {
                EmployeeNo = profile.EmployeeNo,
                StartDate = startOfWeek,
                EndDate = startOfWeek.AddDays(4)
            });

            return (int) response.Workdays.Select(t => t.WorkHours).Sum();
        }
    }
}
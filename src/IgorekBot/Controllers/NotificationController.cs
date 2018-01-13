using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IgorekBot.BLL.Services;
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

        public NotificationController()
        {
            _botSvc = new BotService();
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
                existingConversationMessage.Text = "Не забудь заполнить таймшит за текущую неделю.";
                connector.Conversations.SendToConversation(existingConversationMessage);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
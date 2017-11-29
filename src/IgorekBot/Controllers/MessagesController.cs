using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IgorekBot.BLL.Interfaces;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Common;
using IgorekBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace IgorekBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        private readonly ITimeSheetService _timeSheetService;
        public MessagesController()
        {
            _timeSheetService = new TimeSheetService();
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                /* Creates a dialog stack for the new conversation, adds RootDialog to the stack, and forwards all 
                 *  messages to the dialog stack. */
                await Conversation.SendAsync(activity, () => new RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            
//            if (activity.Type == ActivityTypes.ConversationUpdate)
//            {
//                var userId = Common.CommonConversation.CurrentActivity?.From?.Id;
//                var reslut = _timeSheetService.GetUserById(new GetUserByIdRequest
//                {
//                    ChannelType = (int)ChannelTypes.Telegram,
//                    ChannelId = userId
//                });
//
//                if (reslut.Result == 1)
//                {
//                    await Conversation.SendAsync(activity, () => new Dialogs.AuthenticationDialog());
//                }
//            }
//            else if (activity.Type == ActivityTypes.Message)
//            {
//                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
//            }
//            else
//            {
//                HandleSystemMessage(activity);
//            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Data.Models;
using IgorekBot.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using NMSService.NMSServiceReference;
using Microsoft.Bot.Builder.Internals.Fibers;
using Resources = IgorekBot.Properties.Resources;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly IBotService _botSvc;
        private readonly ITimeSheetService _timeSheetSvc;
        private readonly IEnumerable<string> _mainMenu = new List<string> { Resources.TimeSheetCommand, Resources.EnterAbsenceCommand };
        private UserProfile _profile;

        public RootDialog(IBotService botSvc, ITimeSheetService timeSheetSvc)
        {
            SetField.NotNull(out _botSvc, nameof(botSvc), botSvc);
            SetField.NotNull(out _timeSheetSvc, nameof(timeSheetSvc), timeSheetSvc);
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
       {
           var message = await result;
           
           using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
            {
                if(_profile != null && string.IsNullOrEmpty(_profile.EmployeeNo))
                    context.UserData.RemoveValue("profile");
                if (!context.UserData.TryGetValue("profile", out _profile))
                {
                    _profile = await _botSvc.GetUserProfileByUserId(message.From.Id);
                    if (_profile == null)
                    {
                        context.Call(scope.Resolve<RegistrationDialog>(), ResumeAfterRegistration);
                    }
                    else
                    {
                        context.UserData.SetValue("profile", _profile);
                    }
                }
                else if (message.Text.Equals(Resources.TimeSheetCommand, StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Call(scope.Resolve<TimeSheetDialog>(), ResumeAfterTimeSheetDialog);
                }
                else if (message.Text.Equals(Resources.EnterAbsenceCommand, StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Call(new EnterAbsenceDialog(), ResumeAfterEnterAbsenceDialog);
                }
                else if (message.Text.Equals("/start", StringComparison.InvariantCultureIgnoreCase))
                {
                    await context.PostAsync(MenuHelper.CreateMenu(context, _mainMenu, Resources.RootDialog_Main_Message));
                }
                else
                {
                    await context.PostAsync(MenuHelper.CreateMenu(context, _mainMenu, Resources.RootDialog_Didnt_Understand_Message));
                }
            }
        }

        private async Task ResumeAfterEnterAbsenceDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(MenuHelper.CreateMenu(context, _mainMenu, Resources.RootDialog_Main_Message));
            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAfterTimeSheetDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(MenuHelper.CreateMenu(context, _mainMenu, Resources.RootDialog_Main_Message));
            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAfterRegistration(IDialogContext context, IAwaitable<UserProfile> result)
        {
            try
            {
                _profile = await result;
                if(_profile != null)
                {
                    await _botSvc.SaveUserProfile(_profile);
                    context.UserData.SetValue("profile", _profile);
                    await context.PostAsync(MenuHelper.CreateMenu(context, _mainMenu, string.Format(Resources.RootDialog_Welcome_Message, _profile.FirstName)));
                }
                else
                {
                    await context.PostAsync(MenuHelper.CreateMenu(context,
                        new List<string> { Resources.RegistrationCommand }, Resources.RootDialog_Authentication_Message));
                }

            }
            catch (Exception e)
            {
                await context.PostAsync(MenuHelper.CreateMenu(context,
                    new List<string> { Resources.RegistrationCommand },
                    $"{e.Message}. {Resources.RootDialog_Authentication_Message}"));
            }
            context.Wait(MessageReceivedAsync);
        }

        private async Task RegistrationEnsured(IDialogContext context, IAwaitable<UserProfile> result)
        {
            var profile = await result;

            context.UserData.SetValue(@"profile", profile);

            await context.PostAsync($@"Hello {profile.FirstName}!");

            context.Wait(MessageReceivedAsync);
        }











        //private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        //{
        //    var message = await argument;

        //    var response = _timeSheetSvc.GetUserById(new GetUserByIdRequest { ChannelId = message?.From?.Id });
        //    if (response.Result == 1 || message.Text.Equals(Resources.RegistrationCommand, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        context.Call(new AuthenticationDialog(_timeSheetSvc), AuthenticationDialogResumeAfter);
        //    }
        //    else if (message.Text.Equals(Resources.TimeSheetCommand, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        //await context.Forward(new TimeSheetDialog(_timeSheetSvc), TimeSheetDialogResumeAfterAsync, message);
        //        context.Call(new TimeSheetDialog(_timeSheetSvc), TimeSheetDialogResumeAfterAsync);
        //    }
        //    else if (message.Text.Equals(Resources.EnterAbsenceCommand, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        context.Call(new EnterAbsenceDialog(), EnterAbsenceDialogResumeAfterAsync);
        //    } 
        //    else
        //    {
        //        await context.PostAsync(CreateReply(context, Resources.RootDialog_Didnt_Understand_Message));
        //        context.Wait(MessageReceivedAsync);
        //    }
        //}

        //private async Task TimeSheetDialogResumeAfterAsync(IDialogContext context, IAwaitable<object> result)
        //{
        //    await context.PostAsync(CreateReply(context, Resources.RootDialog_Main_Message));
        //    context.Wait(MessageReceivedAsync);
        //}

        //private async Task EnterAbsenceDialogResumeAfterAsync(IDialogContext context, IAwaitable<object> result)
        //{
        //    var employee = await result;
        //    context.Wait(MessageReceivedAsync);
        //}

        //private async Task SendWelcomeMessageAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        //{
        //}

        //private async Task AuthenticationDialogResumeAfter(IDialogContext context, IAwaitable<Employee> result)
        //{
        //    var employee = await result;
        //    if(employee.FirstName != null)
        //    {
        //        context.UserData.SetValue(@"profile", employee);
        //        await context.PostAsync(CreateReply(context, string.Format(Resources.RootDialog_Welcome_Message, employee.FirstName)));
        //    }
        //    else
        //    {
        //        var reply = context.MakeMessage();
        //        reply.Text = Resources.RootDialog_Authentication_Message;
        //        reply.Type = ActivityTypes.Message;
        //        reply.TextFormat = TextFormatTypes.Plain;

        //        reply.SuggestedActions = new SuggestedActions()
        //        {
        //            Actions = new List<CardAction>
        //            {
        //                new CardAction { Title = Resources.RegistrationCommand, Type=ActionTypes.PostBack, Value = Resources.RegistrationCommand  },
        //            }
        //        };

        //        await context.PostAsync(reply);
        //    }

        //}

        //public static IMessageActivity CreateReply(IDialogContext context, string text)
        //{
        //    var reply = context.MakeMessage();
        //    reply.Text = text;
        //    reply.Type = ActivityTypes.Message;
        //    reply.TextFormat = TextFormatTypes.Plain;

        //    reply.SuggestedActions = new SuggestedActions
        //    {
        //        Actions = new List<CardAction>
        //            {
        //                new CardAction { Title = Resources.TimeSheetCommand, Type=ActionTypes.PostBack, Value = Resources.TimeSheetCommand },
        //                new CardAction { Title = Resources.EnterAbsenceCommand, Type=ActionTypes.PostBack, Value = Resources.TimeSheetCommand },
        //            }
        //    };

        //    return reply;
        //}

        private string x = @"
  First Header  | Second Header
  ------------- | -------------
  Content Cell  | Content Cell
  Content Cell  | Content Cell
";
    }
}
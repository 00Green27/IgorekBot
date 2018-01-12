using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using IgorekBot.BLL.Services;
using IgorekBot.Data.Models;
using IgorekBot.Helpers;
using IgorekBot.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly IBotService _botSvc;

        private readonly IEnumerable<string> _mainMenu =
            new List<string> {Resources.TimeSheetCommand, Resources.EnterAbsenceCommand};

        private readonly ITimeSheetService _timeSheetSvc;
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
                if (_profile != null && string.IsNullOrEmpty(_profile.EmployeeNo))
                    context.UserData.RemoveValue("profile");
                if (!context.UserData.TryGetValue("profile", out _profile))
                {
                    _profile = await _botSvc.GetUserProfileByUserId(message.From.Id);
                    if (_profile == null)
                        context.Call(scope.Resolve<RegistrationDialog>(), ResumeAfterRegistration);
                    else
                        context.UserData.SetValue("profile", _profile);
                }
                else if (message.Text.Equals(Resources.TimeSheetCommand, StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Call(scope.Resolve<TimeSheetDialog>(), ResumeAfterTimeSheetDialog);
                }
                else if (message.Text.Equals(Resources.EnterAbsenceCommand,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Call(new EnterAbsenceDialog(), ResumeAfterEnterAbsenceDialog);
                }
                else if (message.Text.Equals("/start", StringComparison.InvariantCultureIgnoreCase))
                {
                    await context.PostAsync(
                        MenuHelper.CreateMenu(context, _mainMenu, Resources.RootDialog_Main_Message));
                }
                else
                {
                    await context.PostAsync(MenuHelper.CreateMenu(context, _mainMenu,
                        Resources.RootDialog_Didnt_Understand_Message));
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
                if (_profile != null)
                {
                    await _botSvc.SaveUserProfile(_profile);
                    context.UserData.SetValue("profile", _profile);
                    await context.PostAsync(MenuHelper.CreateMenu(context, _mainMenu,
                        string.Format(Resources.RootDialog_Welcome_Message, _profile.FirstName)));
                }
                else
                {
                    await context.PostAsync(MenuHelper.CreateMenu(context,
                        new List<string> {Resources.RegistrationCommand}, Resources.RootDialog_Authentication_Message));
                }
            }
            catch (Exception e)
            {
                await context.PostAsync(MenuHelper.CreateMenu(context,
                    new List<string> {Resources.RegistrationCommand},
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
    }
}
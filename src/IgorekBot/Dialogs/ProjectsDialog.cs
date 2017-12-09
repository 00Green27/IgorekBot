using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;
using IgorekBot.Data.Models;
using IgorekBot.Helpers;
using IgorekBot.Properties;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Internals.Fibers;
using NMSService.NMSServiceReference;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class ProjectsDialog : IDialog<string>
    {

        private readonly ITimeSheetService _service;

        public ProjectsDialog(ITimeSheetService service)
        {
            SetField.NotNull(out _service, nameof(service), service);
        }
        public async Task StartAsync(IDialogContext context)
        {
            var profile = context.UserData.GetValue<UserProfile>("profile");
            var response = _service.GetUserProjects(new GetUserProjectsRequest { UserId = profile.EmployeeCode });
            var projects = response.Projects.ToList();
            var reply = CreateMessageWithHeroCard(context, projects.Select(p => new CardAction { Title = p.ProjectDescription, Value = p.ProjectNo }));
            await context.PostAsync(reply);
            context.Wait<string>(OnProjectSelected);

        }
        
        private async Task OnProjectSelected(IDialogContext context, IAwaitable<string> result)
        {
            var project = await result;
            context.Done(project);
        }


        private static IMessageActivity CreateMessageWithHeroCard(IDialogContext context, IEnumerable<CardAction> actions)
        {
            var reply = MenuHelper.CreateMenu(context, new List<string> { Resources.MenuCommand });

            var projectsCard = new HeroCard
            {
                Text = Resources.TimeSheetDialog_Project_Choice_Message,
                Buttons = actions.ToList()
            };
            reply.Attachments = new List<Attachment>();
            reply.Attachments.Add(projectsCard.ToAttachment());

            return reply;
        }
    }
}
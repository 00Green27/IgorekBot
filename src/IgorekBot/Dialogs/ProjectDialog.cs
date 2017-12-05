using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using IgorekBot.BLL.Interfaces;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace IgorekBot.Dialogs
{
    public class ProjectDialog : IDialog<string>
    {

        private readonly ITimeSheetService _service;

        public ProjectDialog(ITimeSheetService service)
        {
            SetField.NotNull(out _service, nameof(service), service);
        }
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

        }
    }
}
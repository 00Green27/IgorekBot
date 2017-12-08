using Microsoft.Bot.Builder.Dialogs;
using NMSService.NMSServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Internals.Fibers;
using IgorekBot.BLL.Models;
using IgorekBot.BLL.Services;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class EnsureRegistrationDialog : IDialog<Employee>
    {
        private readonly ITimeSheetService _service;

        public EnsureRegistrationDialog(ITimeSheetService service)
        {
            SetField.NotNull(out _service, nameof(service), service);
        }

        public async Task StartAsync(IDialogContext context)
        {
            EnsureRegistration(context);
        }

        private void EnsureRegistration(IDialogContext context)
        {
            var response = _service.GetUserById(new GetUserByIdRequest { ChannelId = context.MakeMessage().From.Id });
        }
    }
}
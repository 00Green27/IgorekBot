using IgorekBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using IgorekBot.BLL.Interfaces;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace IgorekBot.Dialogs
{
    public class RegistrationDialog : IDialog<UserProfile>
    {
        private readonly ITimeSheetService _service;
        private UserProfile _profile;

        public RegistrationDialog(ITimeSheetService service, UserProfile profile)
        {
            SetField.NotNull(out _service, nameof(service), service);
            SetField.NotNull(out _profile, nameof(profile), profile);
        }

        public Task StartAsync(IDialogContext context)
        {
            throw new NotImplementedException();
        }
    }
}
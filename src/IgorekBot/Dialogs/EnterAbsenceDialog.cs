using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class EnterAbsenceDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Fail(new NotImplementedException("Диалоговое окно не реализовано"));
        }
    }
}
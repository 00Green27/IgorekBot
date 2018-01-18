using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IgorekBot.Helpers;
using IgorekBot.Models;
using IgorekBot.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace IgorekBot.Dialogs
{
    [Serializable]
    public class EnterAbsenceDialog : IDialog<object>
    {
        private string _dept;
        private string _type;
        private DateTime _startDate;
        private DateTime _endDate;
        private readonly KeyboardButton[][] _mainMenu = new KeyboardButton[2][]
        {
                    new[] {
                        new KeyboardButton {Text =  Resources.BackCommand, Value = Resources.BackCommand
                        }
                    },
                    new[] {
                        new KeyboardButton {Text =  Resources.CreateAbsenceCommand, Value = Resources.CreateAbsenceCommand}
                    }
        };

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(MenuHelper.CreateMainMenuMessage(context, _mainMenu, "**По оплачиваему отпуску** - 31\n**По больничному без больничного** - 10"));
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (message.Text.Equals(Resources.BackCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Done<object>(null);
            }
            else if (message.Text.Equals(Resources.CreateAbsenceCommand, StringComparison.InvariantCultureIgnoreCase))
            {
                await context.PostAsync(MenuHelper.CreateMenu(context, new List<string>{ Resources.BackCommand }, "Сейчас начнем..."));
                OnCreate(context);
            }
            else
            {
                await context.PostAsync(Resources.RootDialog_Didnt_Understand_Message);
            }
        }

        private void OnCreate(IDialogContext context)
        {
            var list = new List<string>
            {
                "NAV",
                "1C",
                "AX",
                "FPM&BI",
                "CRM",
                "CRM_BANK",
                "Biztalk",
                "Sharepoint",
                "S&M",
                "Админ",
                "PA&OP",
            };

            CancelablePromptChoice<string>.Choice(context, AfterDeptSelected, list,
                "Практика");
        }

        private async Task AfterDeptSelected(IDialogContext context, IAwaitable<string> result)
        {
            _dept = await result;
            if (_dept == null)
            {
                context.Done<object>(null);
                return;
            }

            var list = new List<string>
            {
                "Очередной оплачиваемый отпуск",
                "Отпуск за свой счет",
                "Больничный без больничного",
                "Больничный с больничным",
                "Декрет",
                "Отгул до 4х часов",
            };

            CancelablePromptChoice<string>.Choice(context, AfterTypeSelected, list,
                            "Тип заявки");
        }


        private async Task AfterTypeSelected(IDialogContext context, IAwaitable<string> result)
        {
            _type = await result;
            if (_type == null)
            {
                context.Done<object>(null);
                return;
            }

            var dialog = new PromptDateTime("Дата начала отсутствия (первый день)");
            context.Call(dialog, AfterStartDateEntered);
        }

        private async Task AfterStartDateEntered(IDialogContext context, IAwaitable<DateTime> result)
        {
            _startDate = await result;
            if (_startDate == null)
            {
                context.Done<object>(null);
                return;
            }

            var dialog = new PromptDateTime("Дата окончания отсутствия (последний день)");
            context.Call(dialog, AfterEndDateEntered);
        }

        private async Task AfterEndDateEntered(IDialogContext context, IAwaitable<DateTime> result)
        {
            _endDate = await result;
            if (_endDate == null)
            {
                context.Done<object>(null);
                return;
            }

            await context.PostAsync("Заявка на отсутсвие создана");
            context.Done<object>(null);
        }
    }
}
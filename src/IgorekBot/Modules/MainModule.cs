using System.Configuration;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using IgorekBot.Dialogs;
using IgorekBot.BLL.Services;
using IgorekBot.BLL.Interfaces;

namespace IgorekBot.Modules
{
    public class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<RootDialog>().As<IDialog<object>>().InstancePerDependency();
            
            builder.RegisterType<CancelScorable>().As<IScorable<IActivity, double>>().InstancePerLifetimeScope();

            builder.RegisterType<AuthenticationDialog>().InstancePerDependency();

            builder.RegisterType<TimeSheetService>().Keyed<ITimeSheetService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();

        }
    }
}
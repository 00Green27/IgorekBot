using System.Configuration;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using IgorekBot.Dialogs;
using IgorekBot.BLL.Services;

namespace IgorekBot.Modules
{
    public class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<RootDialog>().As<IDialog<object>>().InstancePerDependency();
            builder.Register(c => new RegistrationDialog(c.Resolve<ITimeSheetService>())).AsSelf().InstancePerDependency();
            builder.Register(c => new TimeSheetDialog(c.Resolve<IBotService>(), c.Resolve<ITimeSheetService>())).AsSelf().InstancePerDependency();

            //builder.RegisterType<MenuScorable>().As<IScorable<IActivity, double>>().InstancePerLifetimeScope();

            builder.RegisterType<AuthenticationDialog>().InstancePerDependency();

            builder.RegisterType<TimeSheetService>().Keyed<ITimeSheetService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<BotService>().Keyed<IBotService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();

        }
    }
}
using Autofac;
using Autofac.Integration.WebApi;
using IgorekBot.Modules;
using Microsoft.Bot.Builder.Dialogs;
using System.Reflection;
using System.Web.Http;

namespace IgorekBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MainModule>();
            builder.Update(Conversation.Container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}

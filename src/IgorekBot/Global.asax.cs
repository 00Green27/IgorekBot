using System;
using System.Configuration;
using Autofac;
using Autofac.Integration.WebApi;
using IgorekBot.Modules;
using Microsoft.Bot.Builder.Dialogs;
using System.Reflection;
using System.Web.Http;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace IgorekBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Conversation.UpdateContainer(
                builder =>
                {
                    builder.RegisterModule<MainModule>();
                    builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));
                    var uri = new Uri(ConfigurationManager.AppSettings["DocumentDBUri"]);
                    var key = ConfigurationManager.AppSettings["DocumentDBKey"];

                    var store = new DocumentDbBotDataStore(uri, key);

                    builder.Register(c => store)
                        .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                        .AsSelf()
                        .SingleInstance();

                    //var store = new TableBotDataStore(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

                    //builder.Register(c => store)
                    //    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    //    .AsSelf()
                    //    .SingleInstance();
                });


            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;

namespace IgorekBot.Common
{
    public static class CommonConversation
    {
        public static ConnectorClient Connector { get; set; }
        public static Activity CurrentActivity { get; set; }
    }
}
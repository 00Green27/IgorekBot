using NMSService.NMSServiceReference;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSService
{
    public static class NMSServiceClientFactory
    {
        public static TimeSheetBotService GetNMSServiceClient()
        {
            var client = new TimeSheetBotService();

            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserName"], ConfigurationManager.AppSettings["Password"], ConfigurationManager.AppSettings["Domain"]);

            return client;
        }
    }
}

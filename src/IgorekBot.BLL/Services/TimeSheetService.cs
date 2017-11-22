using IgorekBot.BLL.Interfaces;
using IgorekBot.BLL.Models;
using NMSService;
using NMSService.NMSServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IgorekBot.BLL.Services
{
    public class TimeSheetService : ITimeSheetService
    {
        public GetUserByIdResponse GetUserById(GetUserByIdRequest request)
        {
            var xmlPort = new root();

            var errText = String.Empty;

            var client = NMSServiceClientFactory.GetNMSServiceClient();

            var result= client.GetEmployeeByID(request.ChannelType, request.ChannelId, ref xmlPort, ref errText);

            var response = new GetUserByIdResponse {
                Result = result,
                ErrorText = errText,
                XMLPort = xmlPort
            };

            return response;
        }
    }
}

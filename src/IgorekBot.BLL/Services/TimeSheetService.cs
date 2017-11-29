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
    [Serializable]
    public class TimeSheetService : ITimeSheetService
    {
        private TimeSheetBotService _client;


        public TimeSheetService()
        {
            _client = NMSServiceClientFactory.GetNMSServiceClient();
        }

        public AddUserByEMailResponse AddUserByEMail(AddUserByEMailRequest request)
        {
            var errText = String.Empty;

            var firstName = String.Empty;

            var lastName = String.Empty;

            var result = _client.AddEmployeeByEMail(request.ChannelType, request.EMail, ref firstName, ref lastName, ref errText);

            var response = new AddUserByEMailResponse
            {
                Result = result,
                ErrorText = errText,
                FirstName = firstName,
                LastName = lastName
            };

            return response;
        }

        public GetProjectTasksResponse GetProjectTasks(GetProjectTasksRequest request)
        {
            var xmlPort = new root5();

            var errText = String.Empty;

            var result = _client.GetEmployeeTasks(request.UserId, request.ProjectId, ref xmlPort, ref errText);

            var response = new GetProjectTasksResponse
            {
                Result = result,
                ErrorText = errText,
                XMLPort = xmlPort
            };

            return response;
        }

        public GetUserByIdResponse GetUserById(GetUserByIdRequest request)
        {
            var xmlPort = new root();

            var errText = String.Empty;

            var result= _client.GetEmployeeByID(request.ChannelType, request.ChannelId, ref xmlPort, ref errText);

            var response = new GetUserByIdResponse
            {
                Result = result,
                ErrorText = errText,
                XMLPort = xmlPort
            };

            return response;
        }

        public GetUserProjectsResponse GetUserProjects(GetUserProjectsRequest request)
        {
            var xmlPort = new root4();

            var errText = String.Empty;

            var result = _client.GetEmployeeProjects(request.UserId, ref xmlPort, ref errText);

            var response = new GetUserProjectsResponse
            {
                Result = result,
                ErrorText = errText,
                XMLPort = xmlPort
            };

            return response;
        }

        public ValidatePasswordResponse ValidatePassword(ValidatePasswordRequest request)
        {
            var xmlPort = new root();

            var errText = String.Empty;

            var result = _client.ValidatePassCode(request.ChannelType, request.EMail, request.Password, request.ChannelId, ref xmlPort, ref errText);

            var response = new ValidatePasswordResponse
            {
                Result = result,
                ErrorText = errText,
                XMLPort = xmlPort
            };

            return response;
        }
    }
}

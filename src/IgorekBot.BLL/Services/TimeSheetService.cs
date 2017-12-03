using IgorekBot.BLL.Interfaces;
using IgorekBot.BLL.Models;
using NMSService;
using NMSService.NMSServiceReference;
using System;


namespace IgorekBot.BLL.Services
{
    public class TimeSheetService : ITimeSheetService
    {

        public AddUserByEMailResponse AddUserByEMail(AddUserByEMailRequest request)
        {
            var errText = String.Empty;

            var firstName = String.Empty;

            var lastName = String.Empty;

            var client = NMSServiceClientFactory.GetNMSServiceClient();

            var result = client.AddEmployeeByEMail((int)request.ChannelType, request.EMail, ref firstName, ref lastName, ref errText);

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

            var client = NMSServiceClientFactory.GetNMSServiceClient();

            var result = client.GetEmployeeTasks(request.UserId, request.ProjectId, ref xmlPort, ref errText);

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

            var client = NMSServiceClientFactory.GetNMSServiceClient();

            var result= client.GetEmployeeByID((int)request.ChannelType, request.ChannelId, ref xmlPort, ref errText);

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

            var client = NMSServiceClientFactory.GetNMSServiceClient();

            var result = client.GetEmployeeProjects(request.UserId, ref xmlPort, ref errText);

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

            var client = NMSServiceClientFactory.GetNMSServiceClient();

            var result = client.ValidatePassCode((int)request.ChannelType, request.EMail, request.Password, request.ChannelId, ref xmlPort, ref errText);

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

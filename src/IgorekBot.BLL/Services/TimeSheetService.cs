using IgorekBot.BLL.Models;
using NMSService;
using NMSService.NMSServiceReference;
using System;
using System.Linq;


namespace IgorekBot.BLL.Services
{
    public class TimeSheetService : ITimeSheetService
    {
        private readonly TimeSheetBotService _client;
        
        public TimeSheetService()
        {
            _client = NMSServiceClientFactory.GetNMSServiceClient();
        }


        public AddUserByEMailResponse AddUserByEmail(AddUserByEmailRequest request)
        {
            var errText = String.Empty;

            var firstName = String.Empty;

            var lastName = String.Empty;

            var result = _client.AddEmployeeByEMail((int)request.ChannelType, request.Email, ref firstName, ref lastName, ref errText);

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
                Tasks = xmlPort.Projects.Select(i => new Task {TaskNo = i.TaskNo, TaskDescription = i.TaskDescription, AssignmentCode = i.AssignmentCode, Description = i.Description})
            };

            return response;
        }

        public AddTimeSheetResponse AddTimeSheet(AddTimeSheetRequest request)
        {
            var errText = String.Empty;

            var result = _client.AddTimeSheet(request.EmployeeNo, request.Date, request.TaskNo, request.AssignmentCode,
                request.Hours, request.Comment, ref errText);

            return new AddTimeSheetResponse
            {
                Result = result,
                ErrorText = errText
            };
        }

        public GetUserByIdResponse GetUserById(GetUserByIdRequest request)
        {
            var xmlPort = new root();

            var errText = String.Empty;
            
            var result= _client.GetEmployeeByID((int)request.ChannelType, request.ChannelId, ref xmlPort, ref errText);

            var response = new GetUserByIdResponse
            {
                Result = result,
                ErrorText = errText,
                Employee = xmlPort.Employee.First()
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
                Projects = xmlPort.Projects
            };

            return response;
        }

        public ValidatePasswordResponse ValidatePassword(ValidatePasswordRequest request)
        {
            var xmlPort = new root();

            var errText = String.Empty;
            
            var result = _client.ValidatePassCode((int)request.ChannelType, request.Email, request.Password, request.ChannelId, ref xmlPort, ref errText);

            var response = new ValidatePasswordResponse
            {
                Result = result,
                ErrorText = errText,
                Employee = xmlPort.Employee.First()
            };

            return response;
        }
    }
}

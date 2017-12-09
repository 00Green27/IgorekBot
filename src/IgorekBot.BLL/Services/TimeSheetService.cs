using System;
using System.Linq;
using IgorekBot.BLL.Models;
using NMSService;
using NMSService.NMSServiceReference;

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
            var errText = string.Empty;

            var firstName = string.Empty;

            var lastName = string.Empty;

            var result = _client.AddEmployeeByEMail((int) request.ChannelType, request.Email, ref firstName,
                ref lastName, ref errText);

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

            var errText = string.Empty;

            var result = _client.GetEmployeeTasks(request.UserId, request.ProjectId, ref xmlPort, ref errText);

            var response = new GetProjectTasksResponse
            {
                Result = result,
                ErrorText = errText,
                ProjectTasks = xmlPort.Projects.Select(i => new ProjectTask
                {
                    TaskNo = i.TaskNo,
                    TaskDescription = i.TaskDescription,
                    AssignmentCode = i.AssignmentCode,
                    Description = i.Description
                })
            };

            return response;
        }

        public GetTimeSheetsPerDayResponse GetTimeSheetsPerDay(GetTimeSheetsPerDayRequest request)
        {
            var xmlPort = new root1();
            var errText = string.Empty;
            var result = _client.TimeSheetsPerDay(request.Date, request.EmployeeNo, ref xmlPort, ref errText);
            return new GetTimeSheetsPerDayResponse
            {
                Result = result,
                ErrorText = errText,
                TimeSeets = xmlPort.TimeSheet.ToList()
            };
        }

        public GetTimeSheetsPerWeekResponse GetTimeSheetsPerWeek(GetTimeSheetsPerWeekRequest request)
        {
            var xmlPort = new root2();
            var errText = string.Empty;
            var result = _client.TimeSheetsPerWeek(request.StartDate, request.EndDate, request.EmployeeNo, ref xmlPort, ref errText);
            return new GetTimeSheetsPerWeekResponse
            {
                Result = result,
                ErrorText = errText,
                TimeSeets = xmlPort.TimeSheet.ToList()
            };
        }

        public AddTimeSheetResponse AddTimeSheet(AddTimeSheetRequest request)
        {
            var errText = string.Empty;

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
            var xmlPort = new root3();

            var errText = string.Empty;

            var result =
                _client.GetEmployeeByID((int) request.ChannelType, request.ChannelId, ref xmlPort, ref errText);

            var response = new GetUserByIdResponse
            {
                Result = result,
                ErrorText = errText,
                Employee = xmlPort.Employee.First()
            };

            return response;
        }

        public GetUserByPhoneResponse GetUserByPhone(GetUserByPhoneRequest request)
        {
            var xmlPort = new root();
            _client.GetEmployeeByPhoneNo(request.Phone, ref xmlPort);
            return new GetUserByPhoneResponse
            {
                Employee = xmlPort.Employee.First()
            };
        }

        public GetUserProjectsResponse GetUserProjects(GetUserProjectsRequest request)
        {
            var xmlPort = new root4();

            var errText = string.Empty;

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
            var xmlPort = new root3();

            var errText = string.Empty;

            var result = _client.ValidatePassCode((int) request.ChannelType, request.Email, request.Password,
                request.ChannelId, ref xmlPort, ref errText);

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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IgorekBot.BLL.Models;
using IgorekBot.Helpers;
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

        public GetTimeSheetsPerWeekResponse GetWorkdays(GetTimeSheetsPerWeekRequest request)
        {
            var xmlPort = new root2();
            var errText = string.Empty;
            var result = _client.TimeSheetsPerWeek(request.StartDate, request.EndDate, request.EmployeeNo, ref xmlPort, ref errText);
            
            var workdays = new List<Workday>();
            if (result != 1)
            {
                var days = xmlPort.TimeSheet
                    .Where(t => Enum.TryParse(t.DayName[0], out DayOfWeek _)  && DateTime.TryParseExact(t.PostingDate[0], "MM/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    .Select(t => new Workday
                    {
                        DayOfWeek = (DayOfWeek) Enum.Parse(typeof(DayOfWeek), t.DayName[0]),
                        Date = DateTime.ParseExact(t.PostingDate[0], "MM/dd/yy", CultureInfo.InvariantCulture),
                        WorkHours = double.TryParse(t.Quantity[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var tmp) ? tmp : 0

                    }).ToList();


                request.StartDate = request.StartDate.AddDays(-1);
                while (request.StartDate.AddDays(1) <= request.EndDate)
                {
                    request.StartDate = request.StartDate.AddDays(1);
                    workdays.Add(days.FirstOrDefault(d => d.Date.Date == request.StartDate.Date) ?? new Workday(request.StartDate));
                }
            }


            return new GetTimeSheetsPerWeekResponse
            {
                Result = result,
                ErrorText = errText,
                Workdays = workdays
            };
        }

        public ServiceResponse AddTimeSheet(AddTimeSheetRequest request, bool doPost)
        {
            var errText = string.Empty;

            var result = _client.AddTimeSheet(request.EmployeeNo, request.Date, request.ProjectNo, request.AssignmentCode,
                request.Hours, request.Comment, ref errText);

            if (string.IsNullOrEmpty(errText) && doPost)
            {
                var startOfWeek = request.Date.StartOfWeek();
                var endOfWeek = startOfWeek.AddDays(6);

                result = _client.PostTimeSheet(request.EmployeeNo, startOfWeek, endOfWeek, ref errText);
            }

            return new ServiceResponse
            {
                Result = result,
                ErrorText = errText
            };
        }

        public ServiceResponse PostTimeSheet(PostTimeSheetRequest request)
        {
            var errText = string.Empty;
            var result = _client.PostTimeSheet(request.EmployeeNo, request.StartDate, request.EndDate, ref errText);

            return new ServiceResponse
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
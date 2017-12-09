using IgorekBot.BLL.Models;

namespace IgorekBot.BLL.Services
{
    public interface ITimeSheetService
    {
        AddUserByEMailResponse AddUserByEmail(AddUserByEmailRequest request);
        AddTimeSheetResponse AddTimeSheet(AddTimeSheetRequest request);
        GetUserByIdResponse GetUserById(GetUserByIdRequest request);
        GetUserByPhoneResponse GetUserByPhone(GetUserByPhoneRequest request);
        GetUserProjectsResponse GetUserProjects(GetUserProjectsRequest request);
        GetProjectTasksResponse GetProjectTasks(GetProjectTasksRequest request);
        GetTimeSheetsPerDayResponse GetTimeSheetsPerDay(GetTimeSheetsPerDayRequest request);
        GetTimeSheetsPerWeekResponse GetTimeSheetsPerWeek(GetTimeSheetsPerWeekRequest request);
        ValidatePasswordResponse ValidatePassword(ValidatePasswordRequest request);


    }
}

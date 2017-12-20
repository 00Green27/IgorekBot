using IgorekBot.BLL.Models;

namespace IgorekBot.BLL.Services
{
    public interface ITimeSheetService
    {
        AddUserByEMailResponse AddUserByEmail(AddUserByEmailRequest request);
        ServiceResponse AddTimeSheet(AddTimeSheetRequest request, bool doPost = false);
        GetUserByIdResponse GetUserById(GetUserByIdRequest request);
        GetUserByPhoneResponse GetUserByPhone(GetUserByPhoneRequest request);
        GetUserProjectsResponse GetUserProjects(GetUserProjectsRequest request);
        GetProjectTasksResponse GetProjectTasks(GetProjectTasksRequest request);
        GetTimeSheetsPerDayResponse GetTimeSheetsPerDay(GetTimeSheetsPerDayRequest request);
        GetTimeSheetsPerWeekResponse GetWorkdays(GetTimeSheetsPerWeekRequest request);
        ValidatePasswordResponse ValidatePassword(ValidatePasswordRequest request);
        ServiceResponse PostTimeSheet(PostTimeSheetRequest request);

    }
}

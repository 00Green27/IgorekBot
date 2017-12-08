using IgorekBot.BLL.Models;

namespace IgorekBot.BLL.Services
{
    public interface ITimeSheetService
    {
        AddUserByEMailResponse AddUserByEmail(AddUserByEmailRequest request);
        AddTimeSheetResponse AddTimeSheet(AddTimeSheetRequest request);
        GetUserByIdResponse GetUserById(GetUserByIdRequest request);
        //GetEmployeeByPhoneNo
        GetUserProjectsResponse GetUserProjects(GetUserProjectsRequest request);
        GetProjectTasksResponse GetProjectTasks(GetProjectTasksRequest request);
        //TimeSheetsPerDay
        //TimeSheetsPerWeek
        ValidatePasswordResponse ValidatePassword(ValidatePasswordRequest request);


    }
}

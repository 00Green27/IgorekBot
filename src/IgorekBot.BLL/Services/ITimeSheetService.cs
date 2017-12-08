using IgorekBot.BLL.Models;

namespace IgorekBot.BLL.Services
{
    public interface ITimeSheetService
    {
        GetUserByIdResponse GetUserById(GetUserByIdRequest request);

        AddUserByEMailResponse AddUserByEmail(AddUserByEmailRequest request);

        ValidatePasswordResponse ValidatePassword(ValidatePasswordRequest request);

        GetUserProjectsResponse GetUserProjects(GetUserProjectsRequest request);

        GetProjectTasksResponse GetProjectTasks(GetProjectTasksRequest request);

        AddTimeSheetResponse AddTimeSheet(AddTimeSheetRequest request);
    }
}

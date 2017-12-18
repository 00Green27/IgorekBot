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
        GetTimeSheetsPerWeekResponse GetWorkdays(GetTimeSheetsPerWeekRequest request);
        ValidatePasswordResponse ValidatePassword(ValidatePasswordRequest request);
//            <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tim="urn:microsoft-dynamics-schemas/codeunit/TimeSheetBotService">
//        <soapenv:Header/>
//        <soapenv:Body>
//        <tim:PostTimeSheet>
//        <tim:employeeNoP>?</tim:employeeNoP>
//        <tim:startDateP>?</tim:startDateP>
//        <tim:endDateP>?</tim:endDateP>
//        <tim:errorText>?</tim:errorText>
//        </tim:PostTimeSheet>
//        </soapenv:Body>
//        </soapenv:Envelope>

    }
}

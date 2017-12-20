using System;

namespace IgorekBot.BLL.Models
{
    public class PostTimeSheetRequest
    {
        public string EmployeeNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    public class PostTimeSheetResponse
    {
    }
}
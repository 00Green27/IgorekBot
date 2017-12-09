using System;

namespace IgorekBot.BLL.Models
{
    public class GetTimeSheetsPerWeekRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EmployeeNo { get; set; }
    }
}
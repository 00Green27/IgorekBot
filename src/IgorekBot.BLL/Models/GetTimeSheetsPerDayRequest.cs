using System;

namespace IgorekBot.BLL.Models
{
    public class GetTimeSheetsPerDayRequest
    {
        public string EmployeeNo { get; set; }
        public DateTime Date { get; set; }
    }
}
using System;

namespace IgorekBot.BLL.Models
{
    public class AddTimeSheetRequest
    {
        public string EmployeeNo { get; set; }
        public DateTime Date { get; set; }
        public string ProjectNo { get; set; }
        public string AssignmentCode { get; set; }
        public decimal Hours { get; set; }
        public string Comment { get; set; }
    }
}
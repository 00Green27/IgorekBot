using System.Collections.Generic;
using NMSService.NMSServiceReference;

namespace IgorekBot.BLL.Models
{
    public class GetTimeSheetsPerWeekResponse
    {
        public int Result { get; set; }
        public string ErrorText { get; set; }
        public IEnumerable<Workday> Workdays { get; set; }
    }
}
using System.Collections.Generic;
using NMSService.NMSServiceReference;

namespace IgorekBot.BLL.Models
{
    public class GetTimeSheetsPerDayResponse
    {
        public int Result { get; set; }
        public string ErrorText { get; set; }
        public IEnumerable<TimeSheet> TimeSeets { get; set; }
    }
}
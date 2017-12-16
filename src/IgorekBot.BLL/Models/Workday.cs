using System;
using System.Globalization;

namespace IgorekBot.BLL.Models
{
    [Serializable]
    public class Workday
    {
        public Workday(DateTime date)
        {
            Date = date;
            DayOfWeek = date.DayOfWeek;
        }

        public Workday()
        {
        }

        public DateTime Date { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int WorkHours { get; set; }

        public override string ToString()
        {
            var ci = new CultureInfo("ru-RU");
            return $"{ci.TextInfo.ToTitleCase(Date.ToString("dddd", ci))} ({WorkHours} ч)"; 
        }
    }
}
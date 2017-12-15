using System;

namespace IgorekBot.Helpers
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, int weekAgo = 0)
        {
            int diff = dt.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff - weekAgo * 7).Date;
        }
    }
}
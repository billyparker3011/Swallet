using System.Globalization;

namespace HnMicro.Core.Helpers
{
    public static class DateHelper
    {
        public static bool IsExpiredDate(this DateTime validateDate)
        {
            var currentDate = DateTime.UtcNow;
            var expiredDate = validateDate.AddDays(30);
            return currentDate < expiredDate;
        }

        public static DateTime? ToDateTime(this string sDateTime, string format)
        {
            if (string.IsNullOrEmpty(sDateTime)) return null;
            if (!DateTime.TryParseExact(sDateTime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime)) return null;
            return dateTime;
        }
    }
}

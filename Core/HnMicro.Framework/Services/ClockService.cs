namespace HnMicro.Framework.Services
{
    public class ClockService : IClockService
    {
        public DateTime GetNow()
        {
            return DateTime.Now;
        }

        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }

        public DateTime ToDateTime(long ticks)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(ticks).ToLocalTime();

            return dateTime;
        }
    }
}

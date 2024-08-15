using Lottery.Core.Configs;

namespace Lottery.Core.Helpers
{
    public static class WinloseCachingHelper
    {
        public static string GetPartOfWinloseMainKey(this DateTime kickOffTime)
        {
            return kickOffTime.ToString("MM-dd-yyyy");
        }

        public static TimeSpan GetDefaultTimeSpan()
        {
            return TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours);
        }

        public static string GetPlayerWinloseMainKey(this int sportKindId, long playerId, string partOfMainKey)
        {
            return string.Format(WinloseCachingConfigs.WinloseBySportKindMainKey, sportKindId, partOfMainKey, playerId / CachingConfigs.HashStructureMaxLength);
        }

        public static string GetPlayerWinloseSubKey(this long playerId)
        {
            return string.Format(WinloseCachingConfigs.WinloseBySportKindSubKey, playerId % CachingConfigs.HashStructureMaxLength);
        }
    }
}

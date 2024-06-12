using HnMicro.Module.Caching.ByRedis.Models;
using Lottery.Core.Configs;

namespace Lottery.Core.Helpers
{
    public static class CachingHelper
    {
        public static string GetSessionKeyByRole(this int roleId, long targetId)
        {
            return string.Format(CachingConfigs.SessionKeyByRole, roleId, targetId);
        }

        public static KeyOfRedisHash GetPlayerPointsByMatchAndNumber(this long playerId, long matchId, int number)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.PlayerPointsByMatchAndNumberKey, playerId / CachingConfigs.HashStructureMaxLength, matchId, number),
                SubKey = string.Format(CachingConfigs.PlayerPointsByMatchAndNumberValueOfKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours)
            };
        }

        public static KeyOfRedisHash GetPlayerOutsByMatch(this long playerId, long matchId)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.PlayerOutsByMatchKey, playerId / CachingConfigs.HashStructureMaxLength, matchId),
                SubKey = string.Format(CachingConfigs.PlayerOutsByMatchValueOfKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours)
            };
        }

        public static KeyOfRedisHash GetPlayerGivenCredit(this long playerId)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.PlayerGivenCreditKey, playerId / CachingConfigs.HashStructureMaxLength),
                SubKey = string.Format(CachingConfigs.PlayerGivenCreditValueOfKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.Zero
            };
        }

        public static KeyOfRedisHash GetMaxPerNumberPlayer(this long playerId, int betKindId)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.MaxPerNumberPlayerKey, playerId / CachingConfigs.HashStructureMaxLength, betKindId),
                SubKey = string.Format(CachingConfigs.MaxPerNumberPlayerValueOfKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.Zero
            };
        }

        public static KeyOfRedisHash GetMaxBetPlayer(this long playerId, int betKindId)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.MaxBetPlayerKey, playerId / CachingConfigs.HashStructureMaxLength, betKindId),
                SubKey = string.Format(CachingConfigs.MaxBetPlayerValueOfKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.Zero
            };
        }

        public static KeyOfRedisHash GetMinBetPlayer(this long playerId, int betKindId)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.MinBetPlayerKey, playerId / CachingConfigs.HashStructureMaxLength, betKindId),
                SubKey = string.Format(CachingConfigs.MinBetPlayerValueOfKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.Zero
            };
        }

        public static KeyOfRedisHash GetPlayerOddByBetKind(this long playerId, int betKindId)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.PlayerOddsByBetKindMainKey, playerId / CachingConfigs.HashStructureMaxLength, betKindId),
                SubKey = string.Format(CachingConfigs.PlayerOddsByBetKindSubKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.Zero
            };
        }

        public static KeyOfRedisHash GetPlayerOddsByMatchBetKindAndNumber(this long playerId, long matchId, int betKindId, int number)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.PlayerOddsByMatchMainKey, playerId / CachingConfigs.HashStructureMaxLength, matchId, betKindId, number),
                SubKey = string.Format(CachingConfigs.PlayerOddsByMatchSubKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours)
            };
        }

        public static KeyOfRedisHash GetMixedPlayerOddsByMatch(this long playerId, long matchId, int originBetKindId, int betKindId)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.MixedPlayerOddsByMatchMainKey, playerId / CachingConfigs.HashStructureMaxLength, matchId, originBetKindId, betKindId),
                SubKey = string.Format(CachingConfigs.MixedPlayerOddsByMatchSubKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours)
            };
        }

        public static KeyOfRedisHash GetPointStatsKeyByMatchBetKindNumber(this long matchId, int betKindId, int number)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.PointStatsKeyByMatchBetKindNumberMainKey, matchId / CachingConfigs.HashStructureMaxLength, betKindId, number),
                SubKey = string.Format(CachingConfigs.PointStatsKeyByMatchBetKindNumberSubKey, matchId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours)
            };
        }

        public static KeyOfRedisHash GetPayoutStatsKeyByMatchBetKindNumber(this long matchId, int betKindId, int number)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.PayoutStatsKeyByMatchBetKindNumberMainKey, matchId / CachingConfigs.HashStructureMaxLength, betKindId, number),
                SubKey = string.Format(CachingConfigs.PayoutStatsKeyByMatchBetKindNumberSubKey, matchId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours)
            };
        }

        public static KeyOfRedisHash GetRateStatsKeyByMatchBetKindNumber(this long matchId, int betKindId, int number)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(CachingConfigs.RateStatsKeyByMatchBetKindNumberMainKey, matchId / CachingConfigs.HashStructureMaxLength, betKindId, number),
                SubKey = string.Format(CachingConfigs.RateStatsKeyByMatchBetKindNumberSubKey, matchId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours)
            };
        }
    }
}

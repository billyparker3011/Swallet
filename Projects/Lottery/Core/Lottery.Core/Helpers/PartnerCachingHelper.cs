﻿using HnMicro.Module.Caching.ByRedis.Models;
using Lottery.Core.Configs;

namespace Lottery.Core.Helpers
{
    public static class PartnerCachingHelper
    {
        public static KeyOfRedisHash GetGa28ClientUrlByPlayerId(this long playerId)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(PartnerCachingConfigs.Ga28ClientUrlByPlayerIdMainKey, playerId / CachingConfigs.HashStructureMaxLength),
                SubKey = string.Format(PartnerCachingConfigs.Ga28ClientUrlByPlayerIdSubKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours)
            };
        }

        public static KeyOfRedisHash GetGa28TokenByPlayerId(this long playerId)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(PartnerCachingConfigs.Ga28TokenByPlayerIdMainKey, playerId / CachingConfigs.HashStructureMaxLength),
                SubKey = string.Format(PartnerCachingConfigs.Ga28TokenByPlayerIdSubKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours)
            };
        }

        public static KeyOfRedisHash GetCasinoClientUrlByPlayerId(this long playerId)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(PartnerCachingConfigs.CasinoClientUrlByPlayerIdMainKey, playerId / CachingConfigs.HashStructureMaxLength),
                SubKey = string.Format(PartnerCachingConfigs.CasinoClientUrlByPlayerIdSubKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours)
            };
        }

        public static KeyOfRedisHash GetCasinoTokenByPlayerId(this long playerId)
        {
            return new KeyOfRedisHash
            {
                MainKey = string.Format(PartnerCachingConfigs.CasinoTokenByPlayerIdMainKey, playerId / CachingConfigs.HashStructureMaxLength),
                SubKey = string.Format(PartnerCachingConfigs.CasinoTokenByPlayerIdSubKey, playerId % CachingConfigs.HashStructureMaxLength),
                TimeSpan = TimeSpan.FromHours(CachingConfigs.ExpiredTimeKeyInHours)
            };
        }
    }
}

﻿using HnMicro.Module.Caching.ByRedis.Options;
using HnMicro.Module.Caching.ByRedis.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HnMicro.Module.Caching.ByRedis.Helpers
{
    public static class CachingByRedisHelper
    {
        public static void BuildRedis(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton(builder.Configuration.GetSection(RedisConfigurationOption.AppSettingName).Get<RedisConfigurationOption>());
            builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
        }

        public static void AddRedis(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton(configuration.GetSection(RedisConfigurationOption.AppSettingName).Get<RedisConfigurationOption>());
            serviceCollection.AddSingleton<IRedisCacheService, RedisCacheService>();
        }
    }
}

using HnMicro.Framework.Logger.Configs;
using HnMicro.Framework.Logger.Helpers;
using HnMicro.Framework.Logger.Models;
using HnMicro.Framework.Options;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HnMicro.Modules.LoggerService.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly IConfiguration _configuration;
        private readonly IClockService _clockService;
        private readonly IRedisCacheService _redisCacheService;

        public LoggerService(IConfiguration configuration, IClockService clockService, IRedisCacheService redisCacheService)
        {
            _configuration = configuration;
            _clockService = clockService;
            _redisCacheService = redisCacheService;
        }

        public async Task Debug(LogRequestModel message)
        {
            await Log(LogLevel.Debug, message);
        }

        public async Task Error(LogRequestModel message)
        {
            await Log(LogLevel.Error, message);
        }

        public async Task Info(LogRequestModel message)
        {
            await Log(LogLevel.Information, message);
        }

        public async Task Warning(LogRequestModel message)
        {
            await Log(LogLevel.Warning, message);
        }

        private async Task Log(LogLevel logLevel, LogRequestModel message)
        {
            if (!IsEnabled(logLevel)) return;

            var serviceOption = _configuration.GetSection(ServiceOption.AppSettingName).Get<ServiceOption>();
            if (serviceOption == null) return;

            await _redisCacheService.PublishAsync(LoggerConfig.LoggerServiceConfigChannel, Newtonsoft.Json.JsonConvert.SerializeObject(new LogModel
            {
                LogLevel = logLevel,
                CategoryName = message.CategoryName,
                Message = message.Message,
                Stacktrace = message.Stacktrace,
                CreatedAt = _clockService.GetUtcNow(),
                CreatedBy = message.CreatedBy,
                RoleId = message.RoleId,
                ServiceCode = serviceOption.Code,
                ServiceName = serviceOption.Name,
                Version = serviceOption.Version
            }), LoggerConfig.LoggerServiceConfigServerName);
        }

        private bool IsEnabled(LogLevel logLevel)
        {
            var configLogLevel = _configuration.GetSection(LoggerHelper.GetDefaultLevelLogger()).Get<LogLevel>();
            return configLogLevel != LogLevel.None && logLevel >= configLogLevel;
        }
    }
}

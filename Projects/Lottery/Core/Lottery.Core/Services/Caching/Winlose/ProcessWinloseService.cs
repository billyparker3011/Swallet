using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Helpers;

namespace Lottery.Core.Services.Caching.Winlose
{
    public class ProcessWinloseService : IProcessWinloseService
    {
        private readonly IRedisCacheService _cacheService;

        public ProcessWinloseService(IRedisCacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task UpdateWinloseCache(Dictionary<string, Dictionary<string, decimal>> items)
        {
            //  Key = MainKey
            //  Value = Dict
            //      Key = Field
            //      Value = Winlose
            foreach (var item in items)
            {
                var fields = item.Value.Keys.ToList();
                var currentData = await _cacheService.HashGetFieldsAsync(item.Key, fields, CachingConfigs.RedisConnectionForApp);
                foreach (var subItem in item.Value)
                {
                    if (!currentData.ContainsKey(subItem.Key)) currentData[subItem.Key] = subItem.Value.ToString();
                    else
                    {
                        var sCurrentDataValue = currentData[subItem.Key];
                        if (string.IsNullOrEmpty(sCurrentDataValue) || !decimal.TryParse(sCurrentDataValue, out decimal currentDataValue)) currentDataValue = 0m;
                        currentDataValue += subItem.Value;
                        currentData[subItem.Key] = currentDataValue.ToString();
                    }
                }
                await _cacheService.HashSetAsync(item.Key, currentData, WinloseCachingHelper.GetDefaultTimeSpan(), CachingConfigs.RedisConnectionForApp);
            }
        }
    }
}

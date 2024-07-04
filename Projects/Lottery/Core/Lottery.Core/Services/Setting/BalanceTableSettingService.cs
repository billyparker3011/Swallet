using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Setting
{
    public class BalanceTableSettingService : LotteryBaseService<BalanceTableSettingService>, IBalanceTableSettingService
    {
        public BalanceTableSettingService(ILogger<BalanceTableSettingService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public string CreateBalanceTableKey(int betKindId)
        {
            return $"DefaultBalanceBetKindTable_{betKindId}";
        }
    }
}

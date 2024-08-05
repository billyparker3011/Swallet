using HnMicro.Framework.Services;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Partners
{
    public class PartnerBaseService<T> : HnMicroBaseService<T>
    {
        protected ILotteryUow LotteryUow;
        public PartnerBaseService(ILogger<T> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService)
        {
            LotteryUow = lotteryUow;
        }
    }
}

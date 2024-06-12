using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services
{
    public class LotteryBaseService<T> : HnMicroBaseService<T>
    {
        protected ILotteryClientContext ClientContext;
        protected ILotteryUow LotteryUow;

        public LotteryBaseService(ILogger<T> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService)
        {
            ClientContext = clientContext;
            LotteryUow = lotteryUow;
        }
    }
}

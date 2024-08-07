using HnMicro.Framework.Services;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Partners
{
    public class PartnerBaseService<T> : HnMicroBaseService<T>
    {
        protected IHttpClientFactory HttpClientFactory;
        protected ILotteryUow LotteryUow;

        public PartnerBaseService(ILogger<T> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, IHttpClientFactory httpClientFactory, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService)
        {
            HttpClientFactory = httpClientFactory;
            LotteryUow = lotteryUow;
        }
    }
}

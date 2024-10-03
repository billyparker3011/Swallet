using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Partners
{
    public class PartnerBaseService<T> : HnMicroBaseService<T>
    {
        protected IHttpClientFactory HttpClientFactory;
        protected ILotteryUow LotteryUow;
        protected IInMemoryUnitOfWork InMemoryUnitOfWork;

        public PartnerBaseService(ILogger<T> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, IHttpClientFactory httpClientFactory, ILotteryUow lotteryUow, IInMemoryUnitOfWork inMemoryUnitOfWork) : base(logger, serviceProvider, configuration, clockService)
        {
            HttpClientFactory = httpClientFactory;
            LotteryUow = lotteryUow;
            InMemoryUnitOfWork = inMemoryUnitOfWork;
        }
    }
}

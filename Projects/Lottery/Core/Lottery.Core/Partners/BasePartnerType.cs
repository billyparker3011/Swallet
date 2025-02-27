﻿using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Enums.Partner;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Partners
{
    public abstract class BasePartnerType<T> : PartnerBaseService<T>, IPartnerService
    {
        protected BasePartnerType(ILogger<T> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, IHttpClientFactory httpClientFactory, ILotteryUow lotteryUow, IInMemoryUnitOfWork inMemoryUnitOfWork) : base(logger, serviceProvider, configuration, clockService, httpClientFactory, lotteryUow, inMemoryUnitOfWork)
        {
        }

        public abstract PartnerType PartnerType { get; set; }

        public abstract Task CreatePlayer(object data);

        public abstract Task UpdateBetSetting(object data);

        public abstract Task GenerateUrl(object data);

        public abstract Task<Dictionary<string, object>> ScanTickets(Dictionary<string, object> metadata);
    }
}

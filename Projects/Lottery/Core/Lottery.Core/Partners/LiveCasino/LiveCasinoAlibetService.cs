﻿using HnMicro.Framework.Services;
using Lottery.Core.Enums.Partner;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Partners.LiveCasino
{
    public class LiveCasinoAlibetService : BasePartnerType
    {
        public LiveCasinoAlibetService(ILogger<BasePartnerType> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, IHttpClientFactory httpClientFactory, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, httpClientFactory, lotteryUow)
        {
        }

        public override PartnerType PartnerType { get; set; } = PartnerType.Allbet;

        public override Task<HttpResponseMessage> CreatePlayer(object data)
        {
            throw new NotImplementedException();
        }

        public override Task GenerateUrl(object data)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateBetSetting(object data)
        {
            throw new NotImplementedException();
        }
    }
}

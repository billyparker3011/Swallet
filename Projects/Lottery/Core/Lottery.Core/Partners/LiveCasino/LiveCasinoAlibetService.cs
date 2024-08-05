using HnMicro.Framework.Services;
using Lottery.Core.Enums.Partner;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Partners.LiveCasino
{
    public class LiveCasinoAlibetService : BasePartnerType
    {
        public override PartnerType PartnerType => PartnerType.Alibet;

        public LiveCasinoAlibetService(ILogger<LiveCasinoAlibetService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, lotteryUow)
        {
        }
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

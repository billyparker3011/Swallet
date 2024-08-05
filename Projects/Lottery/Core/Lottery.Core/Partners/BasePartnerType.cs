using HnMicro.Framework.Services;
using Lottery.Core.Enums.Partner;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Partners
{
    public abstract class BasePartnerType : PartnerBaseService<BasePartnerType>, IPartnerService
    {
        public virtual PartnerType PartnerType { get; set; }

        public BasePartnerType(ILogger<BasePartnerType> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, lotteryUow)
        {

        }
        public abstract Task<HttpResponseMessage> CreatePlayer(object data);

        public abstract Task UpdateBetSetting(object data);

        public abstract Task GenerateUrl(object data);
    }
}

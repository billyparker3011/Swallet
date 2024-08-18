using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoBookieSettingService : LotteryBaseService<CasinoBookieSettingService>, ICasinoBookieSettingService
    {
        public CasinoBookieSettingService(
            ILogger<CasinoBookieSettingService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow)
            : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {

        }

        public async Task<AllbetBookieSettingValue> GetCasinoBookieSettingValueAsync()
        {
            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();
            var bookieSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType.Allbet) ?? throw new NotFoundException();
            return JsonConvert.DeserializeObject<AllbetBookieSettingValue>(bookieSetting.SettingValue);
        }
    }
}

using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;
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

        public async Task CreateCasinoBookieSettingValueAsync(AllbetBookieSettingValue model)
        {
            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();
            var bookieSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType.Allbet);
            if(bookieSetting == null)
            {
                bookieSetting = new BookieSetting()
                {
                    BookieTypeId = PartnerType.Allbet.ToInt(),
                    SettingValue = JsonConvert.SerializeObject(model),
                    CreatedAt = DateTime.Now
                };

               await bookiesSettingRepos.AddAsync(bookieSetting);
            }
            else
            {
                var value = JsonConvert.SerializeObject(model);
                bookieSetting.SettingValue = value;
                bookieSetting.UpdatedAt = DateTime.Now;
                bookiesSettingRepos.Update(bookieSetting);
            }
        

            await LotteryUow.SaveChangesAsync();
        }

        public async Task UpdateCasinoBookieSettingValueAsync(int id, AllbetBookieSettingValue model)
        {
            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();
            var bookieSetting = await bookiesSettingRepos.FindQueryBy(c => c.BookieTypeId == PartnerType.Allbet.ToInt() && c.Id == id).FirstOrDefaultAsync() ?? throw new NotFoundException();

            var value = JsonConvert.SerializeObject(model);
            bookieSetting.SettingValue = value;
            bookieSetting.UpdatedAt = DateTime.Now;
            bookiesSettingRepos.Update(bookieSetting);
            await LotteryUow.SaveChangesAsync();
        }
    }
}

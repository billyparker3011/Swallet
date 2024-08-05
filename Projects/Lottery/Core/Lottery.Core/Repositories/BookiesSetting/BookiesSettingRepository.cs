using HnMicro.Core.Helpers;
using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Enums.Partner;
using Lottery.Data;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.BookiesSetting
{
    public class BookiesSettingRepository : EntityFrameworkCoreRepository<long, Data.Entities.BookieSetting, LotteryContext>, IBookiesSettingRepository
    {
        public BookiesSettingRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<BookieSetting> FindBookieSettingByType(PartnerType type)
        {
            return await FindQuery().FirstOrDefaultAsync(f => f.BookieTypeId == type.ToInt());
        }
    }
}

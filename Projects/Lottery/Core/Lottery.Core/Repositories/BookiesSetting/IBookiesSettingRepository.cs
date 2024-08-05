using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Enums.Partner;
using Lottery.Data;
using Lottery.Data.Entities;

namespace Lottery.Core.Repositories.BookiesSetting
{
    public interface IBookiesSettingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.BookieSetting, LotteryContext>
    {
        Task<BookieSetting> FindBookieSettingByType(PartnerType type);
    }
}

using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public interface ICAPlayerBetSettingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAPlayerBetSetting, LotteryContext>
    {
    }
}

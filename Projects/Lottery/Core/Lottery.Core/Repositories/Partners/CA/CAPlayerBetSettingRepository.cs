using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public class CAPlayerBetSettingRepository: EntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAPlayerBetSetting, LotteryContext>, ICAPlayerBetSettingRepository
    {
        public CAPlayerBetSettingRepository(LotteryContext context) : base(context)
        {
        }

    }
}

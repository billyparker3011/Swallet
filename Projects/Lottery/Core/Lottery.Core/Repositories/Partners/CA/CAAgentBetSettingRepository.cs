using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public class CAAgentBetSettingRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAAgentBetSetting, LotteryContext>, ICAAgentBetSettingRepository
    {
        public CAAgentBetSettingRepository(LotteryContext context) : base(context)
        {
        }

    }
}

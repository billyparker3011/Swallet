using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Bti
{
    public class BtiAgentBetSettingRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Bti.BtiAgentBetSetting, LotteryContext>, IBtiAgentBetSettingRepository
    {
        public BtiAgentBetSettingRepository(LotteryContext context) : base(context)
        {
        }
    }
}

using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoAgentBetSettingAgentHandicapRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoAgentBetSettingAgentHandicap, LotteryContext>, ICasinoAgentBetSettingAgentHandicapRepository
    {
        public CasinoAgentBetSettingAgentHandicapRepository(LotteryContext context) : base(context)
        {
        }

        public void DeleteItems(IEnumerable<Data.Entities.Partners.Casino.CasinoAgentBetSettingAgentHandicap> items)
        {
            foreach (var item in items)
            {
                Delete(item);
            }

            Context.SaveChanges();
        }
    }
}

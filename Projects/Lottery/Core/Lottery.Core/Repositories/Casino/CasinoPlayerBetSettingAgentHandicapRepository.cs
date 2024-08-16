using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoPlayerBetSettingAgentHandicapRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoPlayerBetSettingAgentHandicap, LotteryContext>, ICasinoPlayerBetSettingAgentHandicapRepository
    {
        public CasinoPlayerBetSettingAgentHandicapRepository(LotteryContext context) : base(context)
        {
        }

        public void DeleteItems(IEnumerable<Data.Entities.Partners.Casino.CasinoPlayerBetSettingAgentHandicap> items)
        {
            foreach (var item in items)
            {
                Delete(item);
            }

            Context.SaveChanges();
        }
    }
}

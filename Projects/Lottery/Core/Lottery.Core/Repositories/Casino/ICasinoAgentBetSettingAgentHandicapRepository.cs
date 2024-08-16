using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public interface ICasinoAgentBetSettingAgentHandicapRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoAgentBetSettingAgentHandicap, LotteryContext>
    {
        void DeleteItems(IEnumerable<Data.Entities.Partners.Casino.CasinoAgentBetSettingAgentHandicap> items);
    }
}

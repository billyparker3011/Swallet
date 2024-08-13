using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public interface ICasinoPlayerBetSettingAgentHandicapRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoPlayerBetSettingAgentHandicap, LotteryContext>
    {

    }
}

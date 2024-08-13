using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public interface ICasinoAgentHandicapRepository : IEntityFrameworkCoreRepository<int, Data.Entities.Partners.Casino.CasinoAgentHandicap, LotteryContext>
    {
    }
}

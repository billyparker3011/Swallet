using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public interface ICAAgentHandicapRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAAgentHandicap, LotteryContext>
    {

    }
}

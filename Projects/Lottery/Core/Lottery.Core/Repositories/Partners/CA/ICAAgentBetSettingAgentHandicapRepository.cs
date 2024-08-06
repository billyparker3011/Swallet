using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public interface ICAAgentBetSettingAgentHandicapRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAAgentBetSettingAgentHandicap, LotteryContext>
    {

    }
}

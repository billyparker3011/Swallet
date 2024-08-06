using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public interface ICAPlayerBetSettingAgentHandicapRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAPlayerBetSettingAgentHandicap, LotteryContext>
    {

    }
}

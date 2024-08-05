using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.CockFight
{
    public interface ICockFightAgentBetSettingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.CockFightAgentBetSetting, LotteryContext>
    {

    }
}

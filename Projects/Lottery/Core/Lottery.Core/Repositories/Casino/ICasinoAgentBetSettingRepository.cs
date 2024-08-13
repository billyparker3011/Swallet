using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public interface ICasinoAgentBetSettingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoAgentBetSetting, LotteryContext>
    {

    }
}

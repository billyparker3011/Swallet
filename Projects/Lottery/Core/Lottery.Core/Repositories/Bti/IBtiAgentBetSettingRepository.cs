using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
namespace Lottery.Core.Repositories.Bti
{
    public interface IBtiAgentBetSettingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Bti.BtiAgentBetSetting, LotteryContext>
    {

    }
}

using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public interface ICAAgentBetSettingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAAgentBetSetting, LotteryContext>
    {

    }
}

using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public class CAAgentBetSettingAgentHandicapRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAAgentBetSettingAgentHandicap, LotteryContext>, ICAAgentBetSettingAgentHandicapRepository
    {
        public CAAgentBetSettingAgentHandicapRepository(LotteryContext context) : base(context)
        {
        }
    }
}

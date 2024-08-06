using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public class CAPlayerBetSettingAgentHandicapRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAPlayerBetSettingAgentHandicap, LotteryContext>, ICAPlayerBetSettingAgentHandicapRepository
    {
        public CAPlayerBetSettingAgentHandicapRepository(LotteryContext context) : base(context)
        {
        }

    }
}

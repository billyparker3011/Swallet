using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Channel
{
    public class ChannelRepository : EntityFrameworkCoreRepository<int, Data.Entities.Channel, LotteryContext>, IChannelRepository
    {
        public ChannelRepository(LotteryContext context) : base(context)
        {
        }
    }
}

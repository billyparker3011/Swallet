using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Channel
{
    public interface IChannelRepository : IEntityFrameworkCoreRepository<int, Data.Entities.Channel, LotteryContext>
    {
    }
}

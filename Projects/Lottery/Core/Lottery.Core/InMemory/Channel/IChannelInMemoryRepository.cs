using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Channel;

namespace Lottery.Core.InMemory.Channel
{
    public interface IChannelInMemoryRepository : IInMemoryRepository<int, ChannelModel>
    {
        ChannelModel FindByRegionAndDayOfWeek(int regionId, DayOfWeek dayOfWeek);
        ChannelModel FindByChannelAndRegionAndDayOfWeek(int channelId, int regionId, DayOfWeek dayOfWeek);
    }
}

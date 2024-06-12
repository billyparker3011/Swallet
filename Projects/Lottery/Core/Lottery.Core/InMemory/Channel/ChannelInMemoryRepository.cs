using HnMicro.Core.Helpers;
using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Channel;

namespace Lottery.Core.InMemory.Channel
{
    public class ChannelInMemoryRepository : InMemoryRepository<int, ChannelModel>, IChannelInMemoryRepository
    {
        public ChannelInMemoryRepository()
        {
        }

        public ChannelModel FindByChannelAndRegionAndDayOfWeek(int channelId, int regionId, DayOfWeek dayOfWeek)
        {
            return Items.Values.FirstOrDefault(f => f.Id == channelId && f.RegionId == regionId && f.DayOfWeeks.Contains(dayOfWeek.ToInt()));
        }

        public ChannelModel FindByRegionAndDayOfWeek(int regionId, DayOfWeek dayOfWeek)
        {
            return Items.Values.FirstOrDefault(f => f.RegionId == regionId && f.DayOfWeeks.Contains(dayOfWeek.ToInt()));
        }

        protected override void InternalTryAddOrUpdate(ChannelModel item)
        {
            Items[item.Id] = item;
        }

        protected override void InternalTryRemove(ChannelModel item)
        {
            Items.TryRemove(item.Id, out _);
        }
    }
}

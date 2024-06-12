using Lottery.Core.Models.Channel;
using Lottery.Data.Entities;

namespace Lottery.Core.Helpers.Converters.Channels
{
    public static class ConvertToChannelModel
    {
        public static ChannelModel ToChannelModel(this Channel channel)
        {
            return new ChannelModel
            {
                Id = channel.Id,
                Name = channel.Name,
                RegionId = channel.RegionId,
                DayOfWeeks = channel.DayOfWeeks.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
            };
        }
    }
}

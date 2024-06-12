using HnMicro.Core.Scopes;
using Lottery.Core.Models.Channel;

namespace Lottery.Core.Services.Channel
{
    public interface IChannelService : IScopedDependency
    {
        Task<List<ChannelModel>> GetChannels(int? regionId);
        ChannelFilterOptionModel GetFilterOptions();
        Task UpdateChannels(UpdateChannelsModel model);
    }
}

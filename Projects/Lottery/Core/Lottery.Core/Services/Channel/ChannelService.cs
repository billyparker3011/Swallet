using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Contexts;
using Lottery.Core.InMemory.Region;
using Lottery.Core.Models.Channel;
using Lottery.Core.Repositories.Channel;
using Lottery.Core.Services.Pubs;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Channel
{
    public class ChannelService : LotteryBaseService<ChannelService>, IChannelService
    {
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly IPublishCommonService _publishCommonService;

        public ChannelService(ILogger<ChannelService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IInMemoryUnitOfWork inMemoryUnitOfWork,
            IPublishCommonService publishCommonService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _publishCommonService = publishCommonService;
        }

        public async Task<List<ChannelModel>> GetChannels(int? regionId)
        {
            var channelRepository = LotteryUow.GetRepository<IChannelRepository>();
            var query = channelRepository.FindQuery();
            if (regionId.HasValue) query = query.Where(f => f.RegionId == regionId.Value);
            var data = await query.OrderBy(f => f.RegionId).ThenBy(f => f.Id).ToListAsync();
            return data.Select(f => new ChannelModel
            {
                Id = f.Id,
                RegionId = f.RegionId,
                Name = f.Name,
                DayOfWeeks = string.IsNullOrEmpty(f.DayOfWeeks) ? new List<int>() : f.DayOfWeeks.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(f1 => int.Parse(f1)).OrderBy(f => f).ToList()
            }).ToList();
        }

        public ChannelFilterOptionModel GetFilterOptions()
        {
            var regionRepository = _inMemoryUnitOfWork.GetRepository<IRegionInMemoryRepository>();
            return new ChannelFilterOptionModel
            {
                Regions = regionRepository.GetAll()
            };
        }

        public async Task Refresh()
        {
            var channelRepository = LotteryUow.GetRepository<IChannelRepository>();
            var channels = await channelRepository.FindQueryBy(f => true).ToListAsync();
            var publishedChannels = new List<ChannelModel>();
            channels.ForEach(f =>
            {
                publishedChannels.Add(new ChannelModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    RegionId = f.RegionId,
                    DayOfWeeks = string.IsNullOrEmpty(f.DayOfWeeks) ? new List<int>() : f.DayOfWeeks.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(f1 => int.Parse(f1)).OrderBy(f => f).ToList()
                });
            });

            await _publishCommonService.PublishChannel(publishedChannels);
        }

        public async Task UpdateChannels(UpdateChannelsModel model)
        {
            var channelIds = model.Items.Select(f => f.ChannelId).ToList();
            var channelRepository = LotteryUow.GetRepository<IChannelRepository>();
            var channels = await channelRepository.FindQueryBy(f => channelIds.Contains(f.Id)).ToListAsync();
            var publishedChannels = new List<ChannelModel>();
            channels.ForEach(f =>
            {
                var currentChannel = model.Items.FirstOrDefault(f1 => f1.ChannelId == f.Id);
                if (currentChannel == null) return;

                f.Name = currentChannel.Name;
                channelRepository.Update(f);

                publishedChannels.Add(new ChannelModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    RegionId = f.RegionId,
                    DayOfWeeks = string.IsNullOrEmpty(f.DayOfWeeks) ? new List<int>() : f.DayOfWeeks.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(f1 => int.Parse(f1)).OrderBy(f => f).ToList()
                });
            });

            await LotteryUow.SaveChangesAsync();

            await _publishCommonService.PublishChannel(publishedChannels);
        }
    }
}

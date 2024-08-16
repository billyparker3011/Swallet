using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.InMemory.Category;
using Lottery.Core.InMemory.Channel;
using Lottery.Core.InMemory.Region;
using Lottery.Core.Models.Ticket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Ticket;

public class NormalizeTicketService : HnMicroBaseService<NormalizeTicketService>, INormalizeTicketService
{
    private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;

    public NormalizeTicketService(ILogger<NormalizeTicketService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService,
        IInMemoryUnitOfWork inMemoryUnitOfWork) : base(logger, serviceProvider, configuration, clockService)
    {
        _inMemoryUnitOfWork = inMemoryUnitOfWork;
    }

    public void NormalizePlayer(List<TicketDetailModel> data, Dictionary<long, string> players)
    {
        data.ForEach(f =>
        {
            if (!players.TryGetValue(f.PlayerId, out string username)) return;
            f.Username = username;
        });
    }

    public void NormalizeTicket(List<TicketDetailModel> data)
    {
        var regionInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IRegionInMemoryRepository>();
        var betKindInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
        var categoryInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ICategoryInMemoryRepository>();
        var channelInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
        data.ForEach(f =>
        {
            var region = regionInMemoryRepository.FindById(f.RegionId.ToEnum<Region>());
            f.RegionName = region?.Name;

            var betKind = betKindInMemoryRepository.FindById(f.BetKindId);
            f.BetKindName = betKind?.Name;

            if (betKind != null)
            {
                var category = categoryInMemoryRepository.FindById(betKind.CategoryId.ToEnum<Category>());
                f.CategoryId = betKind.CategoryId;
                f.CategoryName = category?.Name;
            }

            var channel = channelInMemoryRepository.FindById(f.ChannelId);
            f.ChannelName = channel?.Name;

            var splitChooseNumbers = f.ChoosenNumbers.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (f.BetKindId.IsMixedHas2Numbers()) f.ShowMore = splitChooseNumbers.Length > 2;
            else if (f.BetKindId.IsMixedHas3Numbers()) f.ShowMore = splitChooseNumbers.Length > 3;
            else if (f.BetKindId.IsMixedHas4Numbers()) f.ShowMore = splitChooseNumbers.Length > 4;
            else if (f.BetKindId.IsSomeSpecialOfFirstNorthern()) f.ShowMore = true;
            else f.ShowMore = splitChooseNumbers.Length > 1;
            var noOfChooseNumbers = splitChooseNumbers.Length > 9 ? 9 : splitChooseNumbers.Length;
            var arrChooseNumbers = new List<string>();
            for (var i = 0; i < noOfChooseNumbers; i++) arrChooseNumbers.Add(splitChooseNumbers[i].Trim());
            f.ChoosenNumbers = string.Join(", ", arrChooseNumbers);
        });
    }
}
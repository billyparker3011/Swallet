using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using HnMicro.Modules.LoggerService.Services;
using Lottery.Core.Enums;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Odds;
using Lottery.Core.Services.Odds;
using Lottery.Player.OddsService.InMemory.UserOnline;
using Lottery.Player.OddsService.Models;
using Lottery.Player.OddsService.Models.Messages;
using Lottery.Player.OddsService.Services.Token;
using Microsoft.AspNetCore.SignalR;

namespace Lottery.Player.OddsService.Hubs
{
    public class OddsHubHandler : IOddsHubHandler
    {
        private readonly IHubContext<OddsHub, IOddsHubBehavior> _hubContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IClockService _clockService;
        private readonly IInMemoryUnitOfWork _unitOfWork;
        private readonly IReadTokenService _readTokenService;

        public OddsHubHandler(IHubContext<OddsHub, IOddsHubBehavior> hubContext, IServiceProvider serviceProvider, IClockService clockService, IInMemoryUnitOfWork unitOfWork, IReadTokenService readTokenService)
        {
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
            _clockService = clockService;
            _unitOfWork = unitOfWork;
            _readTokenService = readTokenService;
        }

        public async Task CreateConnection(Models.ConnectionInformation connectionInformation)
        {
            var player = _readTokenService.ReadToken(connectionInformation.AccessToken) ?? throw new BadRequestException();
            var userOnlineRepository = _unitOfWork.GetRepository<IUserOnlineInMemoryRepository>();
            var time = _clockService.GetUtcNow();
            var userOnline = new Models.UserOnline
            {
                ConnectionId = connectionInformation.ConnectionId,
                RoleId = player.RoleId,
                ConnectionTime = time,
                PlayerId = player.PlayerId,
                AgentId = player.AgentId,
                MasterId = player.MasterId,
                SupermasterId = player.SupermasterId,
                BetKindId = connectionInformation.BetKindId,
                PongTime = time
            };
            userOnlineRepository.Add(userOnline);

            //await LogConnection(userOnlineRepository, userOnline);

            var oddsMessages = await GetInitialOdds(player.PlayerId, connectionInformation.BetKindId);
            await UpdateOddsByConnectionId(connectionInformation.ConnectionId, oddsMessages);
        }

        private async Task LogConnection(IUserOnlineInMemoryRepository userOnlineRepository, UserOnline userOnline)
        {
            try
            {
                var allUsers = userOnlineRepository.GetAll().ToList();

                using var scope = _serviceProvider.CreateScope();
                var loggerService = scope.ServiceProvider.GetService<ILoggerService>();
                foreach (var user in allUsers)
                {
                    await loggerService.Error(new HnMicro.Framework.Logger.Models.LogRequestModel
                    {
                        CategoryName = nameof(OddsHubHandler),
                        Message = $"{user.ConnectionId} = {user.PlayerId} = {user.ConnectionTime}.",
                        Stacktrace = $"{user.ConnectionId == userOnline.ConnectionId}",
                        RoleId = Role.Player.ToInt(),
                        CreatedBy = user.PlayerId
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async Task UpdateOddsByConnectionId(string connectionId, List<OddsByNumberMessage> oddsMessages)
        {
            await _hubContext.Clients.Client(connectionId).Odds(Newtonsoft.Json.JsonConvert.SerializeObject(oddsMessages));
        }

        private async Task<List<OddsByNumberMessage>> GetInitialOdds(long playerId, int betKindId)
        {
            using var scope = _serviceProvider.CreateScope();
            var oddsService = scope.ServiceProvider.GetService<IOddsService>();
            return (await oddsService.GetInitialOdds(playerId, betKindId)).Select(f => new OddsByNumberMessage
            {
                Number = f.Number,
                BetKinds = f.BetKinds.Select(f1 => new BetKindMessage
                {
                    Id = f1.Id,
                    TotalRate = f1.TotalRate,
                    Buy = f1.Buy
                }).ToList()
            }).ToList();
        }

        public void DeleteConnection(string connectionId)
        {
            var userOnlineRepository = _unitOfWork.GetRepository<IUserOnlineInMemoryRepository>();
            userOnlineRepository.DeleteById(connectionId);
        }

        public async Task UpdateOdds(RateOfOddsValueModel model)
        {
            await _hubContext.Clients.All.UpdateOdds(Newtonsoft.Json.JsonConvert.SerializeObject(new UpdateOddsMessage
            {
                MatchId = model.MatchId,
                BetKindId = model.BetKindId,
                Number = model.Number,
                TotalRate = model.TotalRate
            }));
        }

        public async Task UpdateMixedOdds(MixedRateOfOddsValueModel model)
        {
            await _hubContext.Clients.All.UpdateMixedOdds(Newtonsoft.Json.JsonConvert.SerializeObject(new UpdateMixedOddsMessage
            {
                MatchId = model.MatchId,
                RateValues = model.TotalRate.Select(f => new UpdateMixedOddsDetailMessage
                {
                    BetKindId = f.Key,
                    TotalRate = f.Value
                }).ToList()
            }));
        }

        public async Task StartLive(StartLiveEventModel model)
        {
            await _hubContext.Clients.All.StartLive(Newtonsoft.Json.JsonConvert.SerializeObject(new StartLiveMessage
            {
                MatchId = model.MatchId,
                RegionId = model.RegionId
            }));
        }

        public async Task UpdateMatch(UpdateMatchModel model)
        {
            await _hubContext.Clients.All.UpdateMatch(Newtonsoft.Json.JsonConvert.SerializeObject(new UpdateMatchMessage
            {
                MatchId = model.MatchId
            }));
        }

        public async Task UpdateLiveOdds(UpdateLiveOddsModel model)
        {
            var userOnlineRepository = _unitOfWork.GetRepository<IUserOnlineInMemoryRepository>();
            var players = userOnlineRepository.GetAll().ToList();

            var betKindInMemoryRepository = _unitOfWork.GetRepository<IBetKindInMemoryRepository>();
            var liveBetKindId = betKindInMemoryRepository.GetLiveBetKindByRegion(model.RegionId);
            if (liveBetKindId == 0) return;

            using var scope = _serviceProvider.CreateScope();
            var oddsService = scope.ServiceProvider.GetService<IOddsService>();
            var playerIds = players.Select(f => f.PlayerId).Distinct().ToList();
            var dictOdds = await oddsService.GetLiveOdds(playerIds, liveBetKindId, model.MatchId, model.RegionId, model.ChannelId);
            foreach (var itemPlayer in players)
            {
                if (!dictOdds.TryGetValue(itemPlayer.PlayerId, out LiveOddsModel odds)) continue;
                if (odds.Odds.Count == 0) continue;
                await UpdateLiveOddsByConnectionId(itemPlayer.ConnectionId, odds.NoOfRemainingNumbers, odds.Odds.Select(f => new OddsByNumberMessage
                {
                    Number = f.Number,
                    BetKinds = f.BetKinds.Select(f1 => new BetKindMessage
                    {
                        Id = f1.Id,
                        TotalRate = f1.TotalRate,
                        Buy = f1.Buy
                    }).ToList()
                }).ToList());
            }
        }

        private async Task UpdateLiveOddsByConnectionId(string connectionId, int noOfRemainingNumbers, List<OddsByNumberMessage> oddsMessages)
        {
            await _hubContext.Clients.Client(connectionId).LiveOdds(Newtonsoft.Json.JsonConvert.SerializeObject(new UpdateLiveOddsMessage
            {
                NoOfRemainingNumbers = noOfRemainingNumbers,
                OddsValue = oddsMessages
            }));
        }
    }
}

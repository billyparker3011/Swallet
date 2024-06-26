using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Enums;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Odds;
using Lottery.Core.Services.Match;
using Lottery.Core.Services.Odds;
using Lottery.Core.Services.Others;
using Lottery.Player.OddsService.InMemory.UserOnline;
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
        private readonly INormalizeValueService _normalizeValueService;

        public OddsHubHandler(IHubContext<OddsHub, IOddsHubBehavior> hubContext, IServiceProvider serviceProvider, IClockService clockService, IInMemoryUnitOfWork unitOfWork, IReadTokenService readTokenService, INormalizeValueService normalizeValueService)
        {
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
            _clockService = clockService;
            _unitOfWork = unitOfWork;
            _readTokenService = readTokenService;
            _normalizeValueService = normalizeValueService;
        }

        public async Task CreateConnection(Models.ConnectionInformation connectionInformation)
        {
            var player = _readTokenService.ReadToken(connectionInformation.AccessToken) ?? throw new BadRequestException();
            var userOnlineRepository = _unitOfWork.GetRepository<IUserOnlineInMemoryRepository>();
            var time = _clockService.GetUtcNow();
            userOnlineRepository.Add(new Models.UserOnline
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
            });

            var oddsMessages = await GetOdds(player.PlayerId, connectionInformation.BetKindId);
            await UpdateLiveOddsByConnectionId(connectionInformation.ConnectionId, oddsMessages);
        }

        private async Task UpdateLiveOddsByConnectionId(string connectionId, List<OddsByNumberMessage> oddsMessages)
        {
            await _hubContext.Clients.Client(connectionId).Odds(Newtonsoft.Json.JsonConvert.SerializeObject(oddsMessages));
        }

        private async Task<List<OddsByNumberMessage>> GetOdds(long playerId, int betKindId)
        {
            var betKindIds = betKindId == BetKind.FirstNorthern_Northern_LoXien.ToInt()
                                ? new List<int> { BetKind.FirstNorthern_Northern_Xien2.ToInt(), BetKind.FirstNorthern_Northern_Xien3.ToInt(), BetKind.FirstNorthern_Northern_Xien4.ToInt() }
                                : betKindId == BetKind.FirstNorthern_Northern_LoLive.ToInt()
                                    ? new List<int> { BetKind.FirstNorthern_Northern_Lo.ToInt(), betKindId }
                                    : new List<int> { betKindId };

            using var scope = _serviceProvider.CreateScope();

            var runningMatchService = scope.ServiceProvider.GetService<IRunningMatchService>();
            var runningMatch = await runningMatchService.GetRunningMatch();

            var oddsService = scope.ServiceProvider.GetService<IOddsService>();
            var playerOdds = await oddsService.GetMixedOddsBy(playerId, betKindIds);    //  TODO: Need to read from cache

            var rateOfOddsValue = new Dictionary<int, Dictionary<int, decimal>>();
            if (runningMatch != null)
            {
                var processOddsService = scope.ServiceProvider.GetService<IProcessOddsService>();
                rateOfOddsValue = await processOddsService.GetRateOfOddsValue(runningMatch.MatchId, betKindIds);
            }

            var oddsMessages = new List<OddsByNumberMessage>();
            for (var i = 0; i < 100; i++)
            {
                if (betKindId == BetKind.FirstNorthern_Northern_LoXien.ToInt())
                {
                    var xien2 = playerOdds.FirstOrDefault(f => f.BetKindId == BetKind.FirstNorthern_Northern_Xien2.ToInt());
                    var rateValueOfXien2 = 0m;
                    if (xien2 != null) rateValueOfXien2 = _normalizeValueService.GetRateValue(xien2.BetKindId, i, rateOfOddsValue);

                    var xien3 = playerOdds.FirstOrDefault(f => f.BetKindId == BetKind.FirstNorthern_Northern_Xien3.ToInt());
                    var rateValueOfXien3 = 0m;
                    if (xien3 != null) rateValueOfXien3 = _normalizeValueService.GetRateValue(xien3.BetKindId, i, rateOfOddsValue);

                    var xien4 = playerOdds.FirstOrDefault(f => f.BetKindId == BetKind.FirstNorthern_Northern_Xien4.ToInt());
                    var rateValueOfXien4 = 0m;
                    if (xien4 != null) rateValueOfXien4 = _normalizeValueService.GetRateValue(xien4.BetKindId, i, rateOfOddsValue);

                    oddsMessages.Add(new OddsByNumberMessage
                    {
                        Number = i,
                        BetKinds = new List<BetKindMessage>
                        {
                            new BetKindMessage
                            {
                                Id = BetKind.FirstNorthern_Northern_Xien2.ToInt(),
                                Buy = xien2 == null ? 0m : _normalizeValueService.Normalize(xien2.Buy),
                                TotalRate = xien2 == null ? 0m : _normalizeValueService.Normalize(rateValueOfXien2)
                            },
                            new BetKindMessage
                            {
                                Id = BetKind.FirstNorthern_Northern_Xien3.ToInt(),
                                Buy = xien3 == null ? 0m : _normalizeValueService.Normalize(xien3.Buy),
                                TotalRate = xien3 == null ? 0m : _normalizeValueService.Normalize(rateValueOfXien3)
                            },
                            new BetKindMessage
                            {
                                Id = BetKind.FirstNorthern_Northern_Xien4.ToInt(),
                                Buy = xien4 == null ? 0m : _normalizeValueService.Normalize(xien4.Buy),
                                TotalRate = xien4 == null ? 0m : _normalizeValueService.Normalize(rateValueOfXien4)
                            }
                        }
                    });
                }
                else if (betKindId == BetKind.FirstNorthern_Northern_LoLive.ToInt())
                {
                    var playerOddsForLo = playerOdds.FirstOrDefault(f => f.BetKindId == BetKind.FirstNorthern_Northern_Lo.ToInt());
                    var playerOddsForLoLive = playerOdds.FirstOrDefault(f => f.BetKindId == BetKind.FirstNorthern_Northern_LoLive.ToInt());
                    if (playerOddsForLo == null || playerOddsForLoLive == null) throw new BadRequestException();

                    var rateValueOfLo = _normalizeValueService.GetRateValue(BetKind.FirstNorthern_Northern_Lo.ToInt(), i, rateOfOddsValue);
                    //var rateValueOfLoLive = GetRateValue(BetKind.FirstNorthern_Northern_LoLive.ToInt(), i, rateOfOddsValue);

                    var buyLo = _normalizeValueService.Normalize(playerOddsForLo.Buy) + _normalizeValueService.Normalize(rateValueOfLo);
                    var buyLoLive = _normalizeValueService.Normalize(playerOddsForLoLive.Buy);  // + _normalizeValueService.Normalize(rateValueOfLo);

                    var startBuyLoLive = buyLo;
                    if (startBuyLoLive < buyLoLive) startBuyLoLive = buyLoLive;

                    //  Calculate Buy by Position
                    if (runningMatch != null) startBuyLoLive = _normalizeValueService.Normalize(runningMatchService.GetLiveOdds(betKindId, runningMatch, startBuyLoLive));

                    oddsMessages.Add(new OddsByNumberMessage
                    {
                        Number = i,
                        BetKinds = new List<BetKindMessage>
                        {
                            new BetKindMessage
                            {
                                Id = betKindId,
                                Buy = startBuyLoLive,
                                TotalRate = 0m
                            }
                        }
                    });
                }
                else
                {
                    var currentOdds = playerOdds.FirstOrDefault();
                    var rateValue = _normalizeValueService.GetRateValue(betKindId, i, rateOfOddsValue);
                    oddsMessages.Add(new OddsByNumberMessage
                    {
                        Number = i,
                        BetKinds = new List<BetKindMessage>
                        {
                            new BetKindMessage
                            {
                                Id = betKindId,
                                Buy = currentOdds == null ? 0m : _normalizeValueService.Normalize(currentOdds.Buy),
                                TotalRate = currentOdds == null ? 0m : _normalizeValueService.Normalize(rateValue)
                            }
                        }
                    });
                }
            }
            return oddsMessages;
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

            using var scope = _serviceProvider.CreateScope();

            var runningMatchService = scope.ServiceProvider.GetService<IRunningMatchService>();
            var runningMatch = await runningMatchService.GetRunningMatch();
            if (runningMatch == null) return;
            if (runningMatch.MatchId != model.MatchId) return;
            if (!runningMatch.MatchResult.TryGetValue(model.RegionId, out List<ResultByRegionModel> resultsOfChannel)) return;
            if (!resultsOfChannel.Any(f => f.ChannelId == model.ChannelId && f.IsLive)) return;

            //  Get BetKindIds from RegionId
            var betKindIds = new List<int> { BetKind.FirstNorthern_Northern_Lo.ToInt(), BetKind.FirstNorthern_Northern_LoLive.ToInt() };

            var processOddsService = scope.ServiceProvider.GetService<IProcessOddsService>();
            var rateOfOddsValue = new Dictionary<int, Dictionary<int, decimal>>();
            rateOfOddsValue = await processOddsService.GetRateOfOddsValue(runningMatch.MatchId, betKindIds);

            var oddsService = scope.ServiceProvider.GetService<IOddsService>();
            var playerIds = players.Select(f => f.PlayerId).ToList();
            var allPlayerOdds = await oddsService.GetMixedOddsBy(playerIds, betKindIds);    //  TODO: Need to read from cache

            foreach (var itemPlayer in players)
            {
                var playerOdds = allPlayerOdds.Where(f => f.PlayerId == itemPlayer.PlayerId).ToList();
                var oddsMessages = runningMatchService.GetOddsByPlayerForNorthern(itemPlayer.PlayerId, playerOdds, rateOfOddsValue, runningMatch);
                if (oddsMessages.Count == 0) continue;
                await UpdateLiveOddsByConnectionId(itemPlayer.ConnectionId, oddsMessages.Select(f => new OddsByNumberMessage
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
    }
}

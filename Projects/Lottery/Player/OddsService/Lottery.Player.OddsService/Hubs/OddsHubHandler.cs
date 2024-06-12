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
            await _hubContext.Clients.Client(connectionInformation.ConnectionId).Odds(Newtonsoft.Json.JsonConvert.SerializeObject(oddsMessages));
        }

        private async Task<List<OddsByNumberMessage>> GetOdds(long playerId, int betKindId)
        {
            var betKindIds = betKindId == BetKind.FirstNorthern_Northern_LoXien.ToInt()
                                ? new List<int> { BetKind.FirstNorthern_Northern_Xien2.ToInt(), BetKind.FirstNorthern_Northern_Xien3.ToInt(), BetKind.FirstNorthern_Northern_Xien4.ToInt() }
                                : new List<int> { betKindId };

            using var scope = _serviceProvider.CreateScope();

            var matchService = scope.ServiceProvider.GetService<IMatchService>();
            var runningMatch = await matchService.GetRunningMatch();

            var oddsService = scope.ServiceProvider.GetService<IOddsService>();
            var playerOdds = await oddsService.GetMixedOddsBy(playerId, betKindIds);

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
                    if (xien2 != null) rateValueOfXien2 = GetRateValue(xien2.BetKindId, i, rateOfOddsValue);

                    var xien3 = playerOdds.FirstOrDefault(f => f.BetKindId == BetKind.FirstNorthern_Northern_Xien3.ToInt());
                    var rateValueOfXien3 = 0m;
                    if (xien3 != null) rateValueOfXien3 = GetRateValue(xien3.BetKindId, i, rateOfOddsValue);

                    var xien4 = playerOdds.FirstOrDefault(f => f.BetKindId == BetKind.FirstNorthern_Northern_Xien4.ToInt());
                    var rateValueOfXien4 = 0m;
                    if (xien4 != null) rateValueOfXien4 = GetRateValue(xien4.BetKindId, i, rateOfOddsValue);

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
                else
                {
                    var currentOdds = playerOdds.FirstOrDefault();
                    var rateValue = GetRateValue(betKindId, i, rateOfOddsValue);
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

        private decimal GetRateValue(int betKindId, int number, Dictionary<int, Dictionary<int, decimal>> rateOfOddsValue)
        {
            if (!rateOfOddsValue.TryGetValue(betKindId, out Dictionary<int, decimal> dictRateValue)) return 0m;
            if (!dictRateValue.TryGetValue(number, out decimal rateValue)) return 0m;
            return rateValue;
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

        public async Task StartLive(StartLiveModel model)
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
    }
}

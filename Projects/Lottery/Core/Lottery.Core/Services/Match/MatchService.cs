using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.Channel;
using Lottery.Core.InMemory.Prize;
using Lottery.Core.InMemory.Region;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.Match.ChangeState;
using Lottery.Core.Models.Match.CreateMatch;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Repositories.Match;
using Lottery.Core.Repositories.MatchResult;
using Lottery.Core.Services.Pubs;
using Lottery.Core.Services.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Match
{
    public class MatchService : LotteryBaseService<MatchService>, IMatchService
    {
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ICompletedMatchService _completedMatchService;
        private readonly IPublishCommonService _publishCommonService;

        public MatchService(ILogger<MatchService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IInMemoryUnitOfWork inMemoryUnitOfWork,
            IRedisCacheService redisCacheService,
            ICompletedMatchService completedMatchService,
            IPublishCommonService publishCommonService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _redisCacheService = redisCacheService;
            _completedMatchService = completedMatchService;
            _publishCommonService = publishCommonService;
        }

        public async Task ChangeState(long matchId, ChangeStateModel model)
        {
            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var match = await matchRepository.FindQueryBy(f => f.MatchId == matchId).Include(f => f.MatchResults).FirstOrDefaultAsync() ?? throw new NotFoundException();
            if (model.State == MatchState.Completed.ToInt())
            {
                var prizeInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IPrizeInMemoryRepository>();
                var prizes = prizeInMemoryRepository.GetAll().ToList();

                //  Need check result Region = Northern
                InternalValidationNorthernRegion(match, prizes, Region.Northern);
            }

            match.MatchState = model.State;
            match.UpdatedAt = ClockService.GetUtcNow();
            match.UpdatedBy = ClientContext.Agent.AgentId;
            matchRepository.Update(match);
            await LotteryUow.SaveChangesAsync();

            await PublishUpdateMatch(matchId);
            if (match.MatchState != MatchState.Completed.ToInt()) return;

            _completedMatchService.Enqueue(matchId);
            await _redisCacheService.RemoveAsync(CachingConfigs.RunningMatchKey, CachingConfigs.RedisConnectionForApp);
        }

        private void InternalValidationNorthernRegion(Data.Entities.Match match, List<Models.Prize.PrizeModel> prizes, Region region)
        {
            var regionPrizes = prizes.Where(f => f.RegionId == region.ToInt()).ToList();
            var northernResults = match.MatchResults.Where(f => f.RegionId == region.ToInt()).ToList();
            if (northernResults.Count != 1) throw new BadRequestException(ErrorCodeHelper.MatchChangeState.NorthernHasMoreResult);

            var latestNorthernResults = northernResults.FirstOrDefault();
            if (latestNorthernResults == null || string.IsNullOrEmpty(latestNorthernResults.Results)) throw new BadRequestException(ErrorCodeHelper.MatchChangeState.NorthernResultHasNotUpdatedYet);

            var listPrizes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrizeMatchResultModel>>(latestNorthernResults.Results);
            if (listPrizes.Count == 0) throw new BadRequestException(ErrorCodeHelper.MatchChangeState.NorthernResultHasNotUpdatedYet);
            foreach (var itemPrize in listPrizes.OrderBy(f => f.Prize))
            {
                var currentPrize = regionPrizes.FirstOrDefault(f => f.PrizeId == itemPrize.Prize);
                if (currentPrize == null) throw new BadRequestException(ErrorCodeHelper.MatchChangeState.NorthernResultCannotFindPrize, itemPrize.Prize.ToString());
                if (itemPrize.Results.Count != currentPrize.NoOfNumbers) throw new BadRequestException(ErrorCodeHelper.MatchChangeState.NorthernResultDoesntMatchPrize);
                if (itemPrize.Results.Any(f => string.IsNullOrEmpty(f.Result) || f.Result.Trim().Length < 2)) throw new BadRequestException(ErrorCodeHelper.MatchChangeState.NorthernResultIsBadFormat);
            }
        }

        private async Task PublishUpdateMatch(long matchId)
        {
            await _publishCommonService.PublishUpdateMatch(new UpdateMatchModel
            {
                MatchId = matchId
            });
        }

        public async Task CreateMatch(CreateOrUpdateMatchModel model)
        {
            var vnCurrentTime = ClockService.GetUtcNow().AddHours(7);
            if (!model.AllowBeforeDate && model.KickOff.Date < vnCurrentTime.Date) throw new BadRequestException(ErrorCodeHelper.Match.KickOffIsGreaterOrEqualsCurrentDate);

            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var haveRunning = await matchRepository.HaveRunningOrSuspendedMatch();
            if (haveRunning) throw new BadRequestException(ErrorCodeHelper.Match.RunningMatchIsAlreadyExisted);

            var kickOffTime = model.IncludeTime ? model.KickOff.AddHours(vnCurrentTime.Hour).AddMinutes(vnCurrentTime.Minute).AddSeconds(vnCurrentTime.Second) : model.KickOff;
            var matchCode = model.IncludeTime ? $"{kickOffTime:yyyyMMddHHmmss}" : model.KickOff.ToString("yyyyMMdd");
            var match = await matchRepository.FindByMatchCode(matchCode);
            if (match != null) throw new BadRequestException(ErrorCodeHelper.Match.MatchCodeIsAlreadyExisted);

            match = new Data.Entities.Match
            {
                MatchCode = matchCode,
                KickOffTime = kickOffTime,
                MatchState = MatchState.Running.ToInt(),
                CreatedAt = ClockService.GetUtcNow(),
                CreatedBy = ClientContext.Agent.AgentId
            };
            matchRepository.Add(match);
            await LotteryUow.SaveChangesAsync();

            var matchResults = await GetMatchResults(match.MatchId, match.KickOffTime);
            await CreateOrUpdateRunningMatch(new MatchModel
            {
                MatchId = match.MatchId,
                CreatedAt = match.CreatedAt,
                KickoffTime = match.KickOffTime,
                MatchCode = match.MatchCode,
                MatchResult = matchResults,
                State = match.MatchState
            });

            await PublishUpdateMatch(match.MatchId);
        }

        public async Task UpdateMatch(CreateOrUpdateMatchModel model)
        {
            var vnCurrentTime = ClockService.GetUtcNow().AddHours(7);

            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var match = await matchRepository.FindByIdAsync(model.MatchId) ?? throw new NotFoundException();
            if (!model.AllowBeforeDate)
            {
                if (model.KickOff.Date < vnCurrentTime.Date) throw new BadRequestException();
            }

            var kickOffTime = model.IncludeTime ? model.KickOff.AddHours(vnCurrentTime.Hour).AddMinutes(vnCurrentTime.Minute).AddSeconds(vnCurrentTime.Second) : model.KickOff;
            match.KickOffTime = kickOffTime;
            match.UpdatedAt = ClockService.GetUtcNow();
            match.UpdatedBy = ClientContext.Agent.AgentId;
            matchRepository.Update(match);
            await LotteryUow.SaveChangesAsync();

            await PublishUpdateMatch(match.MatchId);

            if (match.MatchState != MatchState.Running.ToInt()) return;

            var matchResults = await GetMatchResults(match.MatchId, match.KickOffTime);
            await CreateOrUpdateRunningMatch(new MatchModel
            {
                MatchId = match.MatchId,
                CreatedAt = match.CreatedAt,
                KickoffTime = match.KickOffTime,
                MatchCode = match.MatchCode,
                MatchResult = matchResults,
                State = match.MatchState
            });
        }

        public async Task<MatchModel> GetRunningMatch(bool inCache = true)
        {
            if (inCache)
            {
                var dataCache = await _redisCacheService.HashGetAsync(CachingConfigs.RunningMatchKey, CachingConfigs.RedisConnectionForApp);
                if (dataCache == null || dataCache.Count == 0) return null;

                if (!dataCache.TryGetValue(nameof(MatchModel.MatchId), out string sMatchId)) return null;
                if (!long.TryParse(sMatchId, out long matchId) || matchId <= 0L) return null;

                if (!dataCache.TryGetValue(nameof(MatchModel.MatchCode), out string matchCode) || string.IsNullOrEmpty(matchCode)) return null;

                if (!dataCache.TryGetValue(nameof(MatchModel.KickoffTime), out string sKickoffTime)) return null;
                var kickoffTime = sKickoffTime.ToDateTime(CachingConfigs.RedisFormatDateTime);
                if (kickoffTime == null) return null;

                if (!dataCache.TryGetValue(nameof(MatchModel.State), out string sState)) return null;
                if (!int.TryParse(sState, out int state) || state < 0) return null;

                if (!dataCache.TryGetValue(nameof(MatchModel.MatchResult), out string sMatchResults) || string.IsNullOrEmpty(sMatchResults)) return null;
                var matchResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, List<ResultByRegionModel>>>(sMatchResults);
                InternalNormalizeMatchResult(matchResult);

                return new MatchModel
                {
                    MatchId = matchId,
                    MatchCode = matchCode,
                    KickoffTime = kickoffTime.Value,
                    State = state,
                    MatchResult = matchResult
                };
            }

            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var runningMatch = await matchRepository.GetRunningMatch();
            if (runningMatch == null) return null;
            return new MatchModel
            {
                MatchId = runningMatch.MatchId,
                MatchCode = runningMatch.MatchCode,
                KickoffTime = runningMatch.KickOffTime,
                State = runningMatch.MatchState,
                MatchResult = await GetMatchResults(runningMatch.MatchId, runningMatch.KickOffTime)
            };
        }

        private void InternalNormalizeMatchResult(Dictionary<int, List<ResultByRegionModel>> matchResult)
        {
            var prizeInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IPrizeInMemoryRepository>();
            var prizes = prizeInMemoryRepository.GetAll().ToList();
            var groupPrizeByRegion = prizes.GroupBy(f => f.RegionId).Select(f => new
            {
                RegionId = f.Key,
                Prizes = f.OrderBy(f1 => f1.Order).ToList()
            }).ToList();
            foreach (var item in groupPrizeByRegion)
            {
                List<ResultByRegionModel> itemMatchResult;
                if (!matchResult.TryGetValue(item.RegionId, out itemMatchResult)) continue;
                itemMatchResult.ForEach(f =>
                {
                    f.Prize.ForEach(f1 =>
                    {
                        var currentPrize = prizes.FirstOrDefault(f2 => f2.RegionId == item.RegionId && f2.PrizeId == f1.Prize);
                        if (currentPrize == null) return;

                        f1.PrizeName = currentPrize.Name;
                        f1.Order = currentPrize.Order;
                        f1.NoOfNumbers = currentPrize.NoOfNumbers;
                        for (var i = 0; i < f1.NoOfNumbers; i++)
                        {
                            var position = f1.Prize.GetPositionOfPrize(i);
                            var itemPosition = f1.Results.FirstOrDefault(f2 => f2.Position == position);
                            if (itemPosition == null) f1.Results.Add(new PrizeMatchResultDetailModel { Position = position, Result = string.Empty });
                        }
                    });
                });
            }
        }

        private async Task<Dictionary<int, List<ResultByRegionModel>>> GetMatchResults(long matchId, DateTime kickoffTime)
        {
            var dict = new Dictionary<long, DateTime>
            {
                { matchId, kickoffTime }
            };
            var results = await GetMatchResults(dict);
            if (!results.TryGetValue(matchId, out Dictionary<int, List<ResultByRegionModel>> v)) return new Dictionary<int, List<ResultByRegionModel>>();
            return v;
        }

        private async Task<Dictionary<long, Dictionary<int, List<ResultByRegionModel>>>> GetMatchResults(Dictionary<long, DateTime> matches)
        {
            var matchIds = matches.Keys.ToList();

            var matchResultRepository = LotteryUow.GetRepository<IMatchResultRepository>();
            var matchResults = await matchResultRepository.FindByMatchIds(matchIds);

            var regionInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IRegionInMemoryRepository>();
            var regions = regionInMemoryRepository.GetAll().ToList();

            var channelInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
            var channels = channelInMemoryRepository.GetAll().ToList();

            var prizeInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IPrizeInMemoryRepository>();
            var prizes = prizeInMemoryRepository.GetAll().ToList();

            var resultsByMatchId = new Dictionary<long, Dictionary<int, List<ResultByRegionModel>>>();
            foreach (var item in matches)
            {
                var resultByRegion = new Dictionary<int, List<ResultByRegionModel>>();
                foreach (var region in regions)
                {
                    if (!resultByRegion.TryGetValue(region.Id.ToInt(), out List<ResultByRegionModel> regionDetail))
                    {
                        regionDetail = new List<ResultByRegionModel>();
                        resultByRegion[region.Id.ToInt()] = regionDetail;
                    }

                    var channelsOfRegion = channels.Where(f => f.RegionId == region.Id.ToInt() && f.DayOfWeeks.Any(f1 => f1 == (int)item.Value.DayOfWeek)).ToList();
                    foreach (var itemChannel in channelsOfRegion)
                    {
                        var listPrize = new List<PrizeMatchResultModel>();
                        var currentMatchResult = matchResults.FirstOrDefault(f => f.MatchId == item.Key && f.RegionId == region.Id.ToInt() && f.ChannelId == itemChannel.Id);
                        if (currentMatchResult == null || string.IsNullOrEmpty(currentMatchResult.Results))
                        {
                            var regionPrizes = prizes.Where(f => f.RegionId == region.Id.ToInt()).OrderBy(f => f.Id).ToList();
                            regionPrizes.ForEach(f =>
                            {
                                listPrize.Add(new PrizeMatchResultModel
                                {
                                    NoOfNumbers = f.NoOfNumbers,
                                    Order = f.Order,
                                    Prize = f.PrizeId
                                });
                            });
                        }
                        else listPrize = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrizeMatchResultModel>>(currentMatchResult.Results);
                        listPrize.ForEach(f =>
                        {
                            var regionPrizes = prizes.Where(f1 => f1.RegionId == region.Id.ToInt()).ToList();
                            var currentPrize = regionPrizes.FirstOrDefault(f1 => f1.PrizeId == f.Prize);
                            if (currentPrize == null) return;

                            f.PrizeName = currentPrize.Name;
                            f.Order = currentPrize.Order;
                            f.NoOfNumbers = currentPrize.NoOfNumbers;
                            for (var i = 0; i < f.NoOfNumbers; i++)
                            {
                                var position = f.Prize.GetPositionOfPrize(i);
                                var itemPosition = f.Results.FirstOrDefault(f2 => f2.Position == position);
                                if (itemPosition == null) f.Results.Add(new PrizeMatchResultDetailModel { Position = position, Result = string.Empty });
                            }
                        });
                        regionDetail.Add(new ResultByRegionModel
                        {
                            ChannelId = itemChannel.Id,
                            ChannelName = itemChannel.Name,
                            IsLive = currentMatchResult != null && currentMatchResult.IsLive,
                            Prize = listPrize,
                            EnabledProcessTicket = currentMatchResult != null && currentMatchResult.EnabledProcessTicket
                        });
                    }
                }

                resultsByMatchId[item.Key] = resultByRegion;
            }
            return resultsByMatchId;
        }

        private async Task CreateOrUpdateRunningMatch(MatchModel matchModel)
        {
            var entries = new Dictionary<string, string>
            {
                { nameof(MatchModel.MatchId), matchModel.MatchId.ToString() },
                { nameof(MatchModel.MatchCode), matchModel.MatchCode },
                { nameof(MatchModel.KickoffTime), matchModel.KickoffTime.ToString(CachingConfigs.RedisFormatDateTime) },
                { nameof(MatchModel.State), matchModel.State.ToString() },
                { nameof(MatchModel.MatchResult), Newtonsoft.Json.JsonConvert.SerializeObject(matchModel.MatchResult ?? new Dictionary<int, List<ResultByRegionModel>>()) }
            };
            await _redisCacheService.HashSetAsync(CachingConfigs.RunningMatchKey, entries, null, CachingConfigs.RedisConnectionForApp);
        }

        public async Task UpdateMatchResults(MatchResultModel model)
        {
            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var match = await matchRepository.FindByIdAsync(model.MatchId) ?? throw new NotFoundException();

            var regionInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IRegionInMemoryRepository>();
            var region = regionInMemoryRepository.FindById(model.RegionId.ToEnum<Region>()) ?? throw new NotFoundException();

            var channelInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
            var channel = channelInMemoryRepository.FindByChannelAndRegionAndDayOfWeek(model.ChannelId, model.RegionId, match.KickOffTime.DayOfWeek);
            if (channel == null || channel.Id != model.ChannelId) throw new NotFoundException();

            var orderedResults = model.Results.OrderBy(f => f.Prize).ToList();
            var isStartLive = false;

            var matchResultRepository = LotteryUow.GetRepository<IMatchResultRepository>();
            var result = await matchResultRepository.FindByMatchIdAndRegionIdAndChannelId(model.MatchId, model.RegionId, channel.Id);
            if (result == null)
            {
                result = new Data.Entities.MatchResult
                {
                    MatchId = model.MatchId,
                    RegionId = model.RegionId,
                    ChannelId = channel.Id,
                    IsLive = model.IsLive,
                    Results = Newtonsoft.Json.JsonConvert.SerializeObject(orderedResults),
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = ClientContext.Agent.AgentId
                };
                matchResultRepository.Add(result);
                isStartLive = model.IsLive;
            }
            else
            {
                isStartLive = !result.IsLive && model.IsLive;

                result.IsLive = model.IsLive;
                result.Results = Newtonsoft.Json.JsonConvert.SerializeObject(orderedResults);
                result.UpdatedAt = ClockService.GetUtcNow();
                result.UpdatedBy = ClientContext.Agent.AgentId;
                matchResultRepository.Update(result);
            }

            await LotteryUow.SaveChangesAsync();

            var matchResults = await GetMatchResults(match.MatchId, match.KickOffTime);
            await CreateOrUpdateRunningMatch(new MatchModel
            {
                MatchId = match.MatchId,
                CreatedAt = match.CreatedAt,
                KickoffTime = match.KickOffTime,
                MatchCode = match.MatchCode,
                MatchResult = matchResults,
                State = match.MatchState
            });
            if (!isStartLive) return;
            await _publishCommonService.PublishStartLive(new StartLiveModel
            {
                MatchId = match.MatchId,
                RegionId = model.RegionId
            });
        }

        public async Task<ResultModel> ResultsByKickoff(DateTime kickOffTime)
        {
            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var match = await matchRepository.GetMatchByKickoffTime(kickOffTime);
            if (match == null) return new ResultModel();

            var channelInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
            var channels = channelInMemoryRepository.FindBy(f => true).ToList();

            var resultByRegion = new Dictionary<int, List<ResultByRegionModel>>();
            var matchResults = match.MatchResults.OrderBy(f => f.RegionId).ToList();
            foreach (var item in matchResults)
            {
                var itemChannel = channels.FirstOrDefault(f => f.Id == item.ChannelId);

                List<ResultByRegionModel> rs;
                if (!resultByRegion.TryGetValue(item.RegionId, out rs))
                {
                    rs = new List<ResultByRegionModel>();
                    resultByRegion[item.RegionId] = rs;
                }

                rs.Add(new ResultByRegionModel
                {
                    ChannelId = item.ChannelId,
                    ChannelName = itemChannel != null ? itemChannel.Name : string.Empty,
                    Prize = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrizeMatchResultModel>>(item.Results)
                });
            }
            InternalNormalizeMatchResult(resultByRegion);
            return new ResultModel
            {
                MatchId = match.MatchId,
                KickoffTime = match.KickOffTime,
                State = match.MatchState,
                ResultByRegion = resultByRegion
            };
        }

        public async Task<List<MatchModel>> GetMatches(int top = 30, bool displayResult = false)
        {
            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var data = await matchRepository.FindQueryBy(f => true).OrderByDescending(f => f.MatchId).Take(top).Select(f => new MatchModel
            {
                MatchId = f.MatchId,
                KickoffTime = f.KickOffTime,
                MatchCode = f.MatchCode,
                State = f.MatchState,
                CreatedAt = f.CreatedAt
            }).ToListAsync();
            if (!displayResult)
            {
                var dict = data.ToDictionary(f => f.MatchId, f => f.KickoffTime);
                var results = await GetMatchResults(dict);
                data.ForEach(f =>
                {
                    if (!results.TryGetValue(f.MatchId, out Dictionary<int, List<ResultByRegionModel>> v)) v = new Dictionary<int, List<ResultByRegionModel>>();
                    f.MatchResult = v;
                });
            }
            return data;
        }

        public async Task<MatchModel> GetMatchById(long matchId)
        {
            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var match = await matchRepository.FindByIdAsync(matchId);
            if (match == null) return null;
            return new MatchModel
            {
                MatchId = match.MatchId,
                MatchCode = match.MatchCode,
                KickoffTime = match.KickOffTime,
                State = match.MatchState,
                MatchResult = await GetMatchResults(match.MatchId, match.KickOffTime)
            };
        }

        public async Task OnOffProcessTicketOfChannel(OnOffProcessTicketOfChannelModel model)
        {
            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var match = await matchRepository.FindByIdAsync(model.MatchId) ?? throw new NotFoundException();
            if (match.MatchState != MatchState.Running.ToInt()) return;

            var matchResultRepository = LotteryUow.GetRepository<IMatchResultRepository>();
            var matchResult = await matchResultRepository.FindQueryBy(f => f.MatchId == model.MatchId && f.RegionId == model.RegionId && f.ChannelId == model.ChannelId).FirstOrDefaultAsync();
            if (matchResult == null) return;

            matchResult.EnabledProcessTicket = !matchResult.EnabledProcessTicket;
            matchResultRepository.Update(matchResult);
            await LotteryUow.SaveChangesAsync();

            var matchResults = await GetMatchResults(match.MatchId, match.KickOffTime);
            await CreateOrUpdateRunningMatch(new MatchModel
            {
                MatchId = match.MatchId,
                CreatedAt = match.CreatedAt,
                KickoffTime = match.KickOffTime,
                MatchCode = match.MatchCode,
                MatchResult = matchResults,
                State = match.MatchState
            });
        }
    }
}

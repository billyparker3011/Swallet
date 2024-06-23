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

        public async Task CreateMatch(CreateOrUpdateMatchModel model)
        {
            var vnCurrentTime = ClockService.GetUtcNow().AddHours(7);
            if (!model.AllowBeforeDate && model.KickOff.Date < vnCurrentTime.Date) throw new BadRequestException(ErrorCodeHelper.Match.KickOffIsGreaterOrEqualsCurrentDate);

            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var matchResultRepository = LotteryUow.GetRepository<IMatchResultRepository>();

            var haveRunning = await matchRepository.HaveRunningOrSuspendedMatch();
            if (haveRunning) throw new BadRequestException(ErrorCodeHelper.Match.RunningMatchIsAlreadyExisted);

            var kickOffTime = model.IncludeTime ? model.KickOff.AddHours(vnCurrentTime.Hour).AddMinutes(vnCurrentTime.Minute).AddSeconds(vnCurrentTime.Second) : model.KickOff;
            var matchCode = model.IncludeTime ? $"{kickOffTime:yyyyMMddHHmmss}" : model.KickOff.ToString("yyyyMMdd");
            var match = await matchRepository.FindByMatchCode(matchCode);
            if (match != null) throw new BadRequestException(ErrorCodeHelper.Match.MatchCodeIsAlreadyExisted);

            var channelInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
            var currentChannels = channelInMemoryRepository.FindBy(f => f.DayOfWeeks.Contains((int)kickOffTime.DayOfWeek)).ToList();
            var resultsByRegion = new Dictionary<int, List<ResultByRegionModel>>();

            match = new Data.Entities.Match
            {
                MatchCode = matchCode,
                KickOffTime = kickOffTime,
                MatchState = MatchState.Running.ToInt(),
                CreatedAt = ClockService.GetUtcNow(),
                CreatedBy = ClientContext.Agent.AgentId
            };
            matchRepository.Add(match);
            foreach (var item in currentChannels)
            {
                if (!resultsByRegion.TryGetValue(item.RegionId, out List<ResultByRegionModel> resultsByRegionDetail))
                {
                    resultsByRegionDetail = new List<ResultByRegionModel>();
                    resultsByRegion[item.RegionId] = resultsByRegionDetail;
                }
                var defaultResults = CreateDefaultResults(item.RegionId);
                resultsByRegionDetail.Add(new ResultByRegionModel
                {
                    ChannelId = item.Id,
                    ChannelName = item.Name,
                    EnabledProcessTicket = true,
                    IsLive = false,
                    Prize = defaultResults
                });
                matchResultRepository.Add(new Data.Entities.MatchResult
                {
                    RegionId = item.RegionId,
                    ChannelId = item.Id,
                    IsLive = false,
                    EnabledProcessTicket = true,
                    Results = SerializeResults(defaultResults),
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = ClientContext.Agent.AgentId,
                    Match = match
                });
            }
            await LotteryUow.SaveChangesAsync();

            await CreateOrUpdateRunningMatchInCache(new MatchModel
            {
                MatchId = match.MatchId,
                CreatedAt = match.CreatedAt,
                KickoffTime = match.KickOffTime,
                MatchCode = match.MatchCode,
                MatchResult = resultsByRegion,
                State = match.MatchState
            });

            await PublishUpdateMatch(match.MatchId);
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

            _completedMatchService.Enqueue(new Models.Ticket.CompletedMatchInQueueModel
            {
                MatchId = matchId,
                IsDraft = false
            });
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
                        var listPrize = new List<PrizeResultModel>();
                        var currentMatchResult = matchResults.FirstOrDefault(f => f.MatchId == item.Key && f.RegionId == region.Id.ToInt() && f.ChannelId == itemChannel.Id);
                        if (currentMatchResult == null || string.IsNullOrEmpty(currentMatchResult.Results))
                        {
                            var regionPrizes = prizes.Where(f => f.RegionId == region.Id.ToInt()).OrderBy(f => f.Id).ToList();
                            regionPrizes.ForEach(f =>
                            {
                                listPrize.Add(new PrizeResultModel
                                {
                                    NoOfNumbers = f.NoOfNumbers,
                                    Order = f.Order,
                                    Prize = f.PrizeId
                                });
                            });
                        }
                        else listPrize = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrizeResultModel>>(currentMatchResult.Results);
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
                                if (itemPosition == null) f.Results.Add(new PrizeResultDetailModel { Position = position, Result = string.Empty });
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

        private async Task CreateOrUpdateRunningMatchInCache(MatchModel matchModel)
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
                    Prize = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrizeResultModel>>(item.Results)
                });
            }
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

        public async Task StartStopProcessTicket(StartStopProcessTicketModel model)
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
            await CreateOrUpdateRunningMatchInCache(new MatchModel
            {
                MatchId = match.MatchId,
                CreatedAt = match.CreatedAt,
                KickoffTime = match.KickOffTime,
                MatchCode = match.MatchCode,
                MatchResult = matchResults,
                State = match.MatchState
            });
        }

        public async Task StartStopLive(StartStopLiveModel model)
        {
            var matchResultRepository = LotteryUow.GetRepository<IMatchResultRepository>();
            var matchResult = await matchResultRepository.FindByMatchIdAndRegionIdAndChannelId(model.MatchId, model.RegionId, model.ChannelId);
            if (matchResult != null && matchResult.IsLive && ClientContext.Agent.ParentId != 0L) throw new ForbiddenException();
            if (matchResult == null)
            {
                matchResult = new Data.Entities.MatchResult
                {
                    MatchId = model.MatchId,
                    RegionId = model.RegionId,
                    ChannelId = model.ChannelId,
                    IsLive = true,
                    EnabledProcessTicket = true,
                    Results = SerializeResults(CreateDefaultResults(model.RegionId)),
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = ClientContext.Agent.AgentId
                };
                matchResultRepository.Add(matchResult);
            }
            else
            {
                matchResult.IsLive = !matchResult.IsLive;
                matchResult.UpdatedAt = ClockService.GetUtcNow();
                matchResult.UpdatedBy = ClientContext.Agent.AgentId;
                matchResultRepository.Update(matchResult);
            }
            await LotteryUow.SaveChangesAsync();

            await _publishCommonService.PublishStartLive(new StartLiveEventModel
            {
                MatchId = matchResult.MatchId,
                RegionId = model.RegionId
            });
        }

        public async Task UpdateResult(UpdateResultModel model)
        {
            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var match = await matchRepository.FindByIdAsync(model.MatchId) ?? throw new NotFoundException();

            var regionInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IRegionInMemoryRepository>();
            var region = regionInMemoryRepository.FindById(model.RegionId.ToEnum<Region>()) ?? throw new NotFoundException();

            var channelInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
            var channel = channelInMemoryRepository.FindByChannelAndRegionAndDayOfWeek(model.ChannelId, model.RegionId, match.KickOffTime.DayOfWeek);
            if (channel == null || channel.Id != model.ChannelId) throw new NotFoundException();

            var orderedResults = model.Results.OrderBy(f => f.Prize).ToList();
            var defaultResults = CreateDefaultResults(model.RegionId).OrderBy(f => f.Prize).ToList();
            foreach (var item in defaultResults)
            {
                var prizeItem = orderedResults.FirstOrDefault(f => f.Prize == item.Prize) ?? throw new BadRequestException();
                if (item.Results.Count != prizeItem.Results.Count) throw new BadRequestException();
            }
            var detailResults = orderedResults.SelectMany(f => f.Results).OrderBy(f => f.Position).ToList();
            var countDetailResults = detailResults.Count;
            for (var i = 1; i < countDetailResults; i++)
            {
                var previousDetailResult = detailResults[i - 1];
                var currentDetailResult = detailResults[i];
                if (!string.IsNullOrEmpty(currentDetailResult.Result) && string.IsNullOrEmpty(previousDetailResult.Result)) throw new BadRequestException();
            }

            var matchResultRepository = LotteryUow.GetRepository<IMatchResultRepository>();
            var result = await matchResultRepository.FindByMatchIdAndRegionIdAndChannelId(model.MatchId, model.RegionId, channel.Id);
            if (result == null)
            {
                result = new Data.Entities.MatchResult
                {
                    MatchId = model.MatchId,
                    RegionId = model.RegionId,
                    ChannelId = model.ChannelId,
                    IsLive = false,
                    Results = SerializeResults(orderedResults),
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = ClientContext.Agent.AgentId,
                    EnabledProcessTicket = true
                };
                matchResultRepository.Add(result);
            }
            else
            {
                result.Results = SerializeResults(orderedResults);
                result.UpdatedAt = ClockService.GetUtcNow();
                result.UpdatedBy = ClientContext.Agent.AgentId;
                matchResultRepository.Update(result);
            }

            await LotteryUow.SaveChangesAsync();

            var matchResults = await GetMatchResults(match.MatchId, match.KickOffTime);
            //await CreateOrUpdateRunningMatch(new MatchModel
            //{
            //    MatchId = match.MatchId,
            //    CreatedAt = match.CreatedAt,
            //    KickoffTime = match.KickOffTime,
            //    MatchCode = match.MatchCode,
            //    MatchResult = matchResults,
            //    State = match.MatchState
            //});
        }

        private List<PrizeResultModel> CreateDefaultResults(int regionId, bool includePrizeName = false)
        {
            var prizeInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IPrizeInMemoryRepository>();
            var regionPrizes = prizeInMemoryRepository.FindByRegion(regionId);
            var results = new List<PrizeResultModel>();
            regionPrizes.ForEach(f =>
            {
                var detailResults = new List<PrizeResultDetailModel>();
                for (var i = 0; i < f.NoOfNumbers; i++)
                {
                    var position = f.PrizeId.GetPositionOfPrize(i);
                    detailResults.Add(new PrizeResultDetailModel
                    {
                        AllowProcessTicket = true,
                        Position = position,
                        Result = string.Empty
                    });
                }
                results.Add(new PrizeResultModel
                {
                    Prize = f.PrizeId,
                    PrizeName = includePrizeName ? f.Name : null,
                    Order = f.Order,
                    NoOfNumbers = f.NoOfNumbers,
                    Results = detailResults
                });
            });
            return results.OrderBy(f => f.Prize).ToList();
        }

        private string SerializeResults(List<PrizeResultModel> results)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(results);
        }

        #region Obsever
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
            await CreateOrUpdateRunningMatchInCache(new MatchModel
            {
                MatchId = match.MatchId,
                CreatedAt = match.CreatedAt,
                KickoffTime = match.KickOffTime,
                MatchCode = match.MatchCode,
                MatchResult = matchResults,
                State = match.MatchState
            });
            if (!isStartLive) return;
            await _publishCommonService.PublishStartLive(new StartLiveEventModel
            {
                MatchId = match.MatchId,
                RegionId = model.RegionId
            });
        }
        #endregion
    }
}

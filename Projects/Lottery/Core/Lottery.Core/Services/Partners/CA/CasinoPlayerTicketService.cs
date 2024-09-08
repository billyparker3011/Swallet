using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Lottery.Core.Helpers.PartnerHelper;
using static Lottery.Core.Partners.Helpers.CasinoHelper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoPlayerTicketService : LotteryBaseService<CasinoPlayerTicketService>, ICasinoPlayerTicketService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly ICasinoPlayerMappingService _casinoPlayerMappingService;

        public CasinoPlayerTicketService(
           ILogger<CasinoPlayerTicketService> logger,
           IServiceProvider serviceProvider,
           IConfiguration configuration,
           IClockService clockService,
           ILotteryClientContext clientContext,
           ILotteryUow lotteryUow,
           IRedisCacheService redisCacheService,
           ICasinoPlayerMappingService casinoPlayerMappingService)
           : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _redisCacheService = redisCacheService;
            _casinoPlayerMappingService = casinoPlayerMappingService;
        }

        public async Task<List<CasinoBetTableTicketModel>> GetCasinoPlayerOuts(long playerId)
        {
            var casinoBetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();
            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();
            var casinoGameTypeRepository = LotteryUow.GetRepository<ICasinoGameTypeRepository>();

            var betRunning = CasinoBetStatus.BetRunning.ToList();
            var betCompleted = CasinoBetStatus.BetCompleted.ToList();

            var queryTicketCompleted = casinoTicketBetDetailRepository.FindQueryBy(c => betCompleted.Contains(c.Status)).Select(c => c.GameRoundId).Distinct();
            var queryData = casinoTicketBetDetailRepository.FindQueryBy(c => c.CasinoTicket.PlayerId == playerId && !c.IsCancel && betRunning.Contains(c.Status) && !queryTicketCompleted.Contains(c.GameRoundId)).Include(c => c.CasinoTicket).ThenInclude(c=>c.Player).AsQueryable();
            if (ClientContext.Agent.RoleId == Role.Supermaster.ToInt())
            {
                var supermasterId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
                queryData = queryData.Where(f => f.CasinoTicket.SupermasterId == supermasterId);
            }
            else if (ClientContext.Agent.RoleId == Role.Master.ToInt())
            {
                var supermasterId = ClientContext.Agent.SupermasterId;
                var masterId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
                queryData = queryData.Where(f => f.CasinoTicket.SupermasterId == supermasterId && f.CasinoTicket.MasterId == masterId);
            }
            else if (ClientContext.Agent.RoleId == Role.Agent.ToInt())
            {
                var supermasterId = ClientContext.Agent.SupermasterId;
                var masterId = ClientContext.Agent.MasterId;
                var agentId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
                queryData = queryData.Where(f => f.CasinoTicket.SupermasterId == supermasterId && f.CasinoTicket.MasterId == masterId && f.CasinoTicket.AgentId == agentId);
            }

            var datas = await queryData.OrderByDescending(f => f.Id).ToListAsync();

            var betEnums = datas.Select(c => c.GameRoundId).Distinct().ToList();
            var result = new List<CasinoBetTableTicketModel>();
            betEnums.ForEach(c =>
            {
                var details = datas.Where(x => x.GameRoundId == c).ToList();
                var casinoTicketBetDetail = details.FirstOrDefault();
                var betKind = casinoBetKindRepository.FindById(casinoTicketBetDetail.CasinoTicket.BetKindId);
                var casinoTicketBetDetailModels = details.Select(x => ToBetTableTicketDetailModel(x)).ToList();
                result.Add(new CasinoBetTableTicketModel()
                {

                    GameRoundId = c,
                    DateCreated = details.Min(c => c.CreatedAt),
                    TableName = casinoTicketBetDetailModels.FirstOrDefault().GameTypeName + " " + casinoTicketBetDetail.TableName,
                    //BetTypeName = casinoTicketBetDetailModels.FirstOrDefault().BetTypeName,
                    BetAmount = casinoTicketBetDetailModels.Select(c => (c.Status == CasinoBetStatus.Settled && c.Type != CasinoTransferType.Manual_Settle) ? 0m : c.BetAmount).Sum(),
                    WinlossAmountTotal = details.Select(c => c.CasinoTicket.WinlossAmountTotal).Sum(),
                    Ip = casinoTicketBetDetail.Ip,

                    BetKindId = casinoTicketBetDetail.CasinoTicket.BetKindId,
                    BetKindName = betKind?.Name,
                    BookiePlayerId = casinoTicketBetDetail.CasinoTicket.BookiePlayerId,
                    BetNum = casinoTicketBetDetail.BetNum,
                    Currency = casinoTicketBetDetail.CasinoTicket.Currency,
                    PlayerId = casinoTicketBetDetail.CasinoTicket.PlayerId,
                    PlayerName = casinoTicketBetDetail.CasinoTicket.Player?.Username,
                    CasinoTicketBetDetailModels = casinoTicketBetDetailModels,

                });

            });

            return result;
        }

        public async Task<List<CasinoBetTableTicketModel>> GetCasinoPlayerWinloseDetail(GetCasinoPlayerWinlossDetailModel model)
        {
            var casinoBetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();
            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();
            var casinoGameTypeRepository = LotteryUow.GetRepository<ICasinoGameTypeRepository>();
            var betCompleted = CasinoBetStatus.BetCompleted.ToList();

            var queryTicketCompleted = casinoTicketBetDetailRepository.FindQueryBy(c => betCompleted.Contains(c.Status)).Select(c => c.GameRoundId).Distinct();

            var casinoTicketBetDetails = await casinoTicketBetDetailRepository.FindQueryBy(c => c.CasinoTicket.PlayerId == model.PlayerId && !c.IsCancel && c.CreatedAt >= model.FromDate && c.CreatedAt <= model.ToDate).Include(c => c.CasinoTicket).ThenInclude(c=>c.Player).OrderByDescending(c => c.Id).ToListAsync();

            var betEnums = casinoTicketBetDetails.Select(c => c.GameRoundId).Distinct().ToList();
            var result = new List<CasinoBetTableTicketModel>();
            betEnums.ForEach(c =>
            {
                var details = casinoTicketBetDetails.Where(x => x.GameRoundId == c).ToList();
                var casinoTicketBetDetail = details.FirstOrDefault();
                var betKind = casinoBetKindRepository.FindById(casinoTicketBetDetail.CasinoTicket.BetKindId);
                var casinoTicketBetDetailModels = details.Select(x => ToBetTableTicketDetailModel(x)).ToList();
                result.Add(new CasinoBetTableTicketModel()
                {

                    GameRoundId = c,
                    DateCreated = details.Min(c => c.CreatedAt),
                    TableName = casinoTicketBetDetailModels.FirstOrDefault().GameTypeName + " " + casinoTicketBetDetail.TableName,
                    //BetTypeName = casinoTicketBetDetailModels.FirstOrDefault().BetTypeName,
                    BetAmount = casinoTicketBetDetailModels.Select(c => (c.Status == CasinoBetStatus.Settled && c.Type != CasinoTransferType.Manual_Settle) ? 0m : c.BetAmount).Sum(),
                    WinlossAmountTotal = details.Select(c => c.CasinoTicket.WinlossAmountTotal).Sum(),
                    Ip = casinoTicketBetDetail.Ip,

                    BetKindId = casinoTicketBetDetail.CasinoTicket.BetKindId,
                    BetKindName = betKind?.Name,
                    BookiePlayerId = casinoTicketBetDetail.CasinoTicket.BookiePlayerId,
                    BetNum = casinoTicketBetDetail.BetNum,
                    Currency = casinoTicketBetDetail.CasinoTicket.Currency,
                    PlayerId = casinoTicketBetDetail.CasinoTicket.PlayerId,
                    PlayerName = casinoTicketBetDetail.CasinoTicket.Player?.Username,
                    CasinoTicketBetDetailModels = casinoTicketBetDetailModels,

                });

            });

            return result.ToList();
        }

        public CasinoBetTableTicketDetailModel ToBetTableTicketDetailModel(CasinoTicketBetDetail item)
        {
            if (item == null) return new CasinoBetTableTicketDetailModel();
            return new CasinoBetTableTicketDetailModel()
            {
                BetNum = item.BetNum,
                GameRoundId = item.GameRoundId,
                Status = item.Status,
                StatusName = FindStaticFieldName(typeof(CasinoBetStatus), item.Status),
                BetAmount = item.BetAmount,
                Deposit = item.Deposit,
                GameType = item.GameType,
                GameTypeName = FindStaticFieldName(typeof(PartnerHelper.CasinoGameType), item.GameType),
                BetType = item.BetType,
                BetTypeName = FindStaticFieldName(typeof(CasinoBetType), item.BetType),
                Commission = item.Commission,
                ExchangeRate = item.ExchangeRate,
                GameResult = item.GameResult,
                GameResult2 = item.GameResult2,
                WinOrLossAmount = item.WinOrLossAmount,
                ValidAmount = item.ValidAmount,
                BetTime = item.BetTime,
                TableName = item.TableName,
                BetMethod = item.BetMethod,
                BetMethodName = FindStaticFieldName(typeof(CasinoBetMethod), (int)item.BetMethod),
                AppType = item.AppType,
                AppTypeName = FindStaticFieldName(typeof(PartnerHelper.CasinoAppType), (int)item.AppType),
                GameRoundStartTime = item.GameRoundStartTime,
                GameRoundEndTime = item.GameRoundEndTime,
                Type = item.CasinoTicket?.Type ?? 0,
                TypeName = FindStaticFieldName(typeof(PartnerHelper.CasinoTransferType), item.CasinoTicket?.Type ?? 0),
                Ip = item.Ip
            };
        }
    }
}

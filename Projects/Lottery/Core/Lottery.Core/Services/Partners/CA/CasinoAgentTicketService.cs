using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Lottery.Core.Helpers.PartnerHelper;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoAgentTicketService : LotteryBaseService<CasinoAgentTicketService>, ICasinoAgentTicketService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly ICasinoPlayerMappingService _casinoPlayerMappingService;

        public CasinoAgentTicketService(
           ILogger<CasinoAgentTicketService> logger,
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

        public async Task<CasinoAgentTicketModel> GetCasinoLatestTickets(AgentTicketModel model)
        {
            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();

            var casinoTicketBetDetails = casinoTicketBetDetailRepository.FindQuery().Include(c => c.CasinoTicket).ThenInclude(c => c.Player);

            return await PagingCasinoTickets(casinoTicketBetDetailRepository, casinoTicketBetDetails, model);
        }

        public async Task<CasinoAgentTicketModel> GetCasinoRefundRejectTickets(AgentTicketModel model)
        {
            ;
            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();

            var query = casinoTicketBetDetailRepository.FindQuery().Include(c => c.CasinoTicket).ThenInclude(c => c.Player);
            var queryTicketRefund = query.Where(c => CasinoBetStatus.Refund == c.Status).Select(c => c.GameRoundId).Distinct();
            var casinoTicketBetDetails = query.Where(c => queryTicketRefund.Contains(c.GameRoundId));

            return await PagingCasinoTickets(casinoTicketBetDetailRepository, casinoTicketBetDetails, model);
        }

        private async Task<CasinoAgentTicketModel> PagingCasinoTickets(ICasinoTicketBetDetailRepository casinoTicketBetDetailRepository, IQueryable<CasinoTicketBetDetail> casinoTicketBetDetails, AgentTicketModel model)
        {
            var casinoBetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();

            var currentRoleId = ClientContext.Agent.RoleId;
            var currentAgentId = ClientContext.Agent.ParentId == 0 ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;

            if (currentRoleId == Role.Company.ToInt()) casinoTicketBetDetails = casinoTicketBetDetails.Where(f => 1 == 1);
            else if (currentRoleId == Role.Supermaster.ToInt()) casinoTicketBetDetails = casinoTicketBetDetails.Where(f => f.CasinoTicket.SupermasterId == currentAgentId);
            else if (currentRoleId == Role.Master.ToInt()) casinoTicketBetDetails = casinoTicketBetDetails.Where(f => f.CasinoTicket.MasterId == currentAgentId);
            else casinoTicketBetDetails = casinoTicketBetDetails.Where(f => f.CasinoTicket.AgentId == currentAgentId && !f.IsCancel);

            var datas = await casinoTicketBetDetailRepository.PagingByAsync(casinoTicketBetDetails.OrderByDescending(c => c.Id), model.PageIndex, model.PageSize);

            var betEnums = datas.Items.Select(c => c.GameRoundId).Distinct().ToList();
            var result = new List<CasinoBetTableTicketModel>();
            betEnums.ForEach(c =>
            {
                var details = datas.Items.Where(x => x.GameRoundId == c).ToList();
                var casinoTicketBetDetail = details.FirstOrDefault();
                var betKind = casinoBetKindRepository.FindById(casinoTicketBetDetail.CasinoTicket.BetKindId);
                var casinoTicketBetDetailModels = details.Select(x => ToBetTableTicketDetailModel(x)).ToList();
                result.Add(new CasinoBetTableTicketModel()
                {

                    GameRoundId = c,
                    DateCreated = details.Min(c => c.CreatedAt),
                    TableName = casinoTicketBetDetailModels.FirstOrDefault().GameTypeName + " " + casinoTicketBetDetail.TableName,
                    BetTypeName = casinoTicketBetDetailModels.FirstOrDefault().BetTypeName,
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

            return new CasinoAgentTicketModel
            {
                Items = result,
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfPages = datas.Metadata.NoOfPages,
                    NoOfRows = datas.Metadata.NoOfRows,
                    NoOfRowsPerPage = datas.Metadata.NoOfRowsPerPage,
                    Page = datas.Metadata.Page
                }
            };
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

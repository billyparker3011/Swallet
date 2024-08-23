using HnMicro.Core.Helpers;
using HnMicro.Framework.Helpers;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.CockFight;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Helpers.Converters.Partners;
using Lottery.Core.Models.CockFight.GetCockFightPlayerWinlossDetail;
using Lottery.Core.Partners.Helpers;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.CockFight
{
    public class CockFightPlayerTicketService : LotteryBaseService<CockFightPlayerTicketService>, ICockFightPlayerTicketService
    {
        public CockFightPlayerTicketService(ILogger<CockFightPlayerTicketService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public Task<List<CockFightPlayerTicketDetailDto>> GetCockFightDetailTicket(long ticketId, bool fromPlayer = true)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CockFightPlayerTicketDetailDto>> GetCockFightPlayerOuts(long playerId)
        {
            var cockFightTicketRepository = LotteryUow.GetRepository<ICockFightTicketRepository>();
            var outsState = CommonHelper.OutsCockFightTicketState();
            var queryData = cockFightTicketRepository.FindQueryBy(f => f.PlayerId == playerId && !f.ParentId.HasValue && outsState.Contains(f.Status));
            if (ClientContext.Agent.RoleId == Role.Supermaster.ToInt())
            {
                var supermasterId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
                queryData = queryData.Where(f => f.SupermasterId == supermasterId);
            }
            else if (ClientContext.Agent.RoleId == Role.Master.ToInt())
            {
                var supermasterId = ClientContext.Agent.SupermasterId;
                var masterId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
                queryData = queryData.Where(f => f.SupermasterId == supermasterId && f.MasterId == masterId);
            }
            else if (ClientContext.Agent.RoleId == Role.Agent.ToInt())
            {
                var supermasterId = ClientContext.Agent.SupermasterId;
                var masterId = ClientContext.Agent.MasterId;
                var agentId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
                queryData = queryData.Where(f => f.SupermasterId == supermasterId && f.MasterId == masterId && f.AgentId == agentId);
            }
            var datas = await queryData
                .OrderByDescending(f => f.Id)
                .Select(f => new CockFightPlayerTicketDetailDto
                {
                    PlayerId = f.PlayerId,
                    TicketId = f.Sid,
                    BetKindId = f.BetKindId,
                    FightNumber = f.FightNumber,
                    Odds = f.Odds ?? 0m,
                    BetAmount = f.BetAmount ?? 0m,
                    MatchDayCode = f.MatchDayCode,
                    Result = f.Result.ConvertTicketResult(),
                    Selection = f.Selection.ToCockFightSelection(),
                    Status = f.Status.ConvertTicketStatus(),
                    TicketAmount = f.TicketAmount ?? 0m,
                    WinlossAmount = f.WinlossAmount ?? 0m,
                    IpAddress = f.IpAddress,
                    UserAgent = string.IsNullOrEmpty(f.UserAgent) ? f.UserAgent.GetPlatform() : f.UserAgent,
                    ArenaCode = f.ArenaCode,
                    CurrencyCode = f.CurrencyCode,
                    DateCreated = f.TicketCreatedDate,
                    OddsType = f.OddsType
                }).ToListAsync();
            return datas;
        }

        public async Task<List<CockFightPlayerTicketDetailDto>> GetCockFightPlayerWinloseDetail(GetCockFightPlayerWinlossDetailModel model)
        {
            var cockFightTicketRepository = LotteryUow.GetRepository<ICockFightTicketRepository>();
            var completedState = CommonHelper.CompletedCockFightTicketWithoutRefundOrRejectState();
            var datas = await cockFightTicketRepository.FindQueryBy(f => f.PlayerId == model.PlayerId && !f.ParentId.HasValue && f.TicketModifiedDate.HasValue && f.TicketModifiedDate.Value.Date >= model.FromDate.Date && f.TicketModifiedDate.Value.Date <= model.ToDate.Date && completedState.Contains(f.Status))
                .OrderByDescending(f => f.Id)
                .Select(f => new CockFightPlayerTicketDetailDto
                {
                    PlayerId = f.PlayerId,
                    TicketId = f.Sid,
                    BetKindId = f.BetKindId,
                    FightNumber = f.FightNumber,
                    Odds = f.Odds ?? 0m,
                    BetAmount = f.BetAmount ?? 0m,
                    MatchDayCode = f.MatchDayCode,
                    Result = f.Result.ConvertTicketResult(),
                    Selection = f.Selection.ToCockFightSelection(),
                    Status = f.Status.ConvertTicketStatus(),
                    TicketAmount = f.TicketAmount ?? 0m,
                    WinlossAmount = f.WinlossAmount ?? 0m,
                    IpAddress = f.IpAddress,
                    UserAgent = string.IsNullOrEmpty(f.UserAgent) ? f.UserAgent.GetPlatform() : f.UserAgent,
                    ArenaCode = f.ArenaCode,
                    CurrencyCode = f.CurrencyCode,
                    DateCreated = f.TicketCreatedDate,
                    OddsType = f.OddsType
                }).ToListAsync();
            return datas;
        }

        public async Task<List<CockFightPlayerTicketDetailDto>> GetCockFightRefundRejectTickets()
        {
            var cockFightTicketRepository = LotteryUow.GetRepository<ICockFightTicketRepository>();
            var refundRejectState = CommonHelper.RefundRejectCockFightTicketState();
            var lastestMatchDayCodeTicket = await cockFightTicketRepository.FindQueryBy(x => x.PlayerId == ClientContext.Player.PlayerId && !x.ParentId.HasValue).OrderByDescending(x => x.TicketCreatedDate).FirstOrDefaultAsync();
            if (lastestMatchDayCodeTicket == null) return new List<CockFightPlayerTicketDetailDto>();
            var datas = await cockFightTicketRepository.FindQueryBy(f => f.PlayerId == ClientContext.Player.PlayerId && !f.ParentId.HasValue && f.MatchDayCode == lastestMatchDayCodeTicket.MatchDayCode && refundRejectState.Contains(f.Status))
                .OrderByDescending(f => f.Id)
                .Select(f => new CockFightPlayerTicketDetailDto
                {
                    PlayerId = f.PlayerId,
                    TicketId = f.Sid,
                    BetKindId = f.BetKindId,
                    FightNumber = f.FightNumber,
                    Odds = f.Odds ?? 0m,
                    BetAmount = f.BetAmount ?? 0m,
                    MatchDayCode = f.MatchDayCode,
                    Result = f.Result.ConvertTicketResult(),
                    Selection = f.Selection.ToCockFightSelection(),
                    Status = f.Status.ConvertTicketStatus(),
                    TicketAmount = f.TicketAmount ?? 0m,
                    WinlossAmount = f.WinlossAmount ?? 0m,
                    IpAddress = f.IpAddress,
                    UserAgent = string.IsNullOrEmpty(f.UserAgent) ? f.UserAgent.GetPlatform() : f.UserAgent,
                    ArenaCode = f.ArenaCode,
                    CurrencyCode = f.CurrencyCode,
                    DateCreated = f.TicketCreatedDate,
                    OddsType = f.OddsType
                }).ToListAsync();
            return datas;
        }

        public async Task<List<CockFightPlayerTicketDetailDto>> GetCockFightTicketsAsBetList()
        {
            var cockFightTicketRepository = LotteryUow.GetRepository<ICockFightTicketRepository>();
            var runningState = CommonHelper.OutsCockFightTicketState();
            var datas = await cockFightTicketRepository.FindQueryBy(f => f.PlayerId == 6 && !f.ParentId.HasValue && runningState.Contains(f.Status))
                .OrderByDescending(f => f.Id)
                .Select(f => new CockFightPlayerTicketDetailDto
                {
                    PlayerId = f.PlayerId,
                    TicketId = f.Sid,
                    BetKindId = f.BetKindId,
                    FightNumber = f.FightNumber,
                    Odds = f.Odds ?? 0m,
                    BetAmount = f.BetAmount ?? 0m,
                    MatchDayCode = f.MatchDayCode,
                    Result = f.Result.ConvertTicketResult(),
                    Selection = f.Selection.ToCockFightSelection(),
                    Status = f.Status.ConvertTicketStatus(),
                    TicketAmount = f.TicketAmount ?? 0m,
                    WinlossAmount = f.WinlossAmount ?? 0m,
                    IpAddress = f.IpAddress,
                    UserAgent = string.IsNullOrEmpty(f.UserAgent) ? f.UserAgent.GetPlatform() : f.UserAgent,
                    ArenaCode = f.ArenaCode,
                    CurrencyCode = f.CurrencyCode,
                    DateCreated = f.TicketCreatedDate,
                    OddsType = f.OddsType
                }).ToListAsync();
            return datas;
        }

        public Task<List<CockFightPlayerTicketDetailDto>> GetCockFightTicketsByMatch(long matchId)
        {
            throw new NotImplementedException();
        }
    }
}

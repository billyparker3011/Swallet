using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Helpers;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.CockFight;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Helpers.Converters.Partners;
using Lottery.Core.Models.CockFight.GetCockFightAgentTicket;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Partners.Helpers;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.CockFight;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.CockFight
{
    public class CockFightAgentTicketService : LotteryBaseService<CockFightAgentTicketService>, ICockFightAgentTicketService
    {
        public CockFightAgentTicketService(ILogger<CockFightAgentTicketService> logger, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IClockService clockService, 
            ILotteryClientContext clientContext, 
            ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<CockFightAgentTicketResult> CockFightLatestTickets(AgentTicketModel model)
        {
            var cockFightTicketRepository = LotteryUow.GetRepository<ICockFightTicketRepository>();
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();

            var targetAgentId = ClientContext.Agent.ParentId != 0L ? ClientContext.Agent.ParentId : ClientContext.Agent.AgentId;
            var clientAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            var lastestMatchDayCodeTicket = await GetLastestMatchDayCodeTicketByAgentRole(cockFightTicketRepository, clientAgent);
            if (lastestMatchDayCodeTicket == null) return new CockFightAgentTicketResult();
            var rejectRefundState = CommonHelper.RefundRejectTicketState();
            return await InternalGetCockFightTickets(rejectRefundState, model, lastestMatchDayCodeTicket.MatchDayCode);
        }

        private async Task<CockFightTicket> GetLastestMatchDayCodeTicketByAgentRole(ICockFightTicketRepository cockFightTicketRepository, Data.Entities.Agent clientAgent)
        {
            switch (clientAgent.RoleId)
            {
                case (int)Role.Company:
                    return await cockFightTicketRepository.FindQueryBy(x => !x.ParentId.HasValue).OrderByDescending(x => x.TicketCreatedDate).FirstOrDefaultAsync();
                case (int)Role.Supermaster:
                    return await cockFightTicketRepository.FindQueryBy(x => x.SupermasterId == clientAgent.AgentId && !x.ParentId.HasValue).OrderByDescending(x => x.TicketCreatedDate).FirstOrDefaultAsync();
                case (int)Role.Master:
                    return await cockFightTicketRepository.FindQueryBy(x => x.MasterId == clientAgent.AgentId && !x.ParentId.HasValue).OrderByDescending(x => x.TicketCreatedDate).FirstOrDefaultAsync();
                case (int)Role.Agent:
                    return await cockFightTicketRepository.FindQueryBy(x => x.AgentId == clientAgent.AgentId && !x.ParentId.HasValue).OrderByDescending(x => x.TicketCreatedDate).FirstOrDefaultAsync();
                default:
                    return null;
            }
        }

        public async Task<CockFightAgentTicketResult> GetCockFightRefundRejectTickets(AgentTicketModel model)
        {
            var runningState = CommonHelper.OutsTicketState();
            return await InternalGetCockFightTickets(runningState, model);
        }

        private async Task<CockFightAgentTicketResult> InternalGetCockFightTickets(List<int> stateIds, AgentTicketModel model, string matchDayCode = null)
        {
            var currentRoleId = ClientContext.Agent.RoleId;
            var currentAgentId = ClientContext.Agent.ParentId == 0 ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;

            var cockFightTicketRepository = LotteryUow.GetRepository<ICockFightTicketRepository>();
            var cockFightAgentTicketQuery = cockFightTicketRepository.FindQueryBy(f => !f.ParentId.HasValue && stateIds.Contains(f.Status));
            if (!string.IsNullOrEmpty(matchDayCode)) cockFightAgentTicketQuery.Where(f => f.MatchDayCode == matchDayCode);

            if (currentRoleId == Role.Company.ToInt()) cockFightAgentTicketQuery = cockFightAgentTicketQuery.Where(f => 1 == 1);
            else if (currentRoleId == Role.Supermaster.ToInt()) cockFightAgentTicketQuery = cockFightAgentTicketQuery.Where(f => f.SupermasterId == currentAgentId);
            else if (currentRoleId == Role.Master.ToInt()) cockFightAgentTicketQuery = cockFightAgentTicketQuery.Where(f => f.MasterId == currentAgentId);
            else cockFightAgentTicketQuery = cockFightAgentTicketQuery.Where(f => f.AgentId == currentAgentId);

            cockFightAgentTicketQuery = cockFightAgentTicketQuery.OrderByDescending(f => f.CreatedAt);

            var result = await cockFightTicketRepository.PagingByAsync(cockFightAgentTicketQuery, model.PageIndex, model.PageSize);
            var tickets = result.Items.Select(f => new CockFightPlayerTicketDetailDto
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
            }).ToList();

            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var listPlayerId = tickets.Select(f => f.PlayerId).Distinct().ToList();
            var players = await playerRepository.FindQueryBy(f => listPlayerId.Contains(f.PlayerId)).ToListAsync();
            var dictPlayer = players.ToDictionary(f => f.PlayerId, f => f.Username);
            tickets.ForEach(f =>
            {
                if (!dictPlayer.TryGetValue(f.PlayerId, out string username)) return;
                f.Username = username;
            });
            return new CockFightAgentTicketResult
            {
                Items = tickets,
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfPages = result.Metadata.NoOfPages,
                    NoOfRows = result.Metadata.NoOfRows,
                    NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                    Page = result.Metadata.Page
                }
            };
        }
    }
}

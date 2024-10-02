using HnMicro.Framework.Enums;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.BroadCaster;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.BroadCaster.GetBroadCasterOutstanding;
using Lottery.Core.Models.BroadCaster.GetBroadCasterWinlossSummary;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.BetKind;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Lottery.Core.Services.BroadCaster
{
    public class BroadCasterService : LotteryBaseService<BroadCasterService>, IBroadCasterService
    {
        public BroadCasterService(ILogger<BroadCasterService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<GetBroadCasterOutstandingResult> GetBroadCasterOutstandings(GetBroadCasterOutstandingModel model)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();
            var ticketRepos = LotteryUow.GetRepository<ITicketRepository>();
            var listBroadCasterOustanding = new List<BroadCasterOutstandingDto>();
            var targetAgentId = ClientContext.Agent.ParentId == 0L
                                        ? ClientContext.Agent.AgentId
                                        : ClientContext.Agent.ParentId;
            var outsState = CommonHelper.OutsTicketState();
            switch (ClientContext.Agent.RoleId)
            {
                case (int)Role.Company:
                    var companyTicketQuery = ticketRepos.FindQueryBy(tk => outsState.Contains(tk.State));
                    listBroadCasterOustanding = await GetBroadCasterOutstandings(companyTicketQuery, betKindRepos, model);
                    break;
                case (int)Role.Supermaster:
                    var superMasterTicketQuery = ticketRepos.FindQueryBy(tk => tk.SupermasterId == targetAgentId && outsState.Contains(tk.State));
                    listBroadCasterOustanding = await GetBroadCasterOutstandings(superMasterTicketQuery, betKindRepos, model);
                    break;
                case (int)Role.Master:
                    var masterTicketQuery = ticketRepos.FindQueryBy(tk => tk.MasterId == targetAgentId && outsState.Contains(tk.State));
                    listBroadCasterOustanding = await GetBroadCasterOutstandings(masterTicketQuery, betKindRepos, model);
                    break;
                case (int)Role.Agent:
                    var agentTicketQuery = ticketRepos.FindQueryBy(tk => tk.AgentId == targetAgentId && outsState.Contains(tk.State));
                    listBroadCasterOustanding = await GetBroadCasterOutstandings(agentTicketQuery, betKindRepos, model);
                    break;
                default:
                    break;
            }

            return new GetBroadCasterOutstandingResult
            {
                BroadCasterOuts = listBroadCasterOustanding,
                SummaryBetCount = listBroadCasterOustanding.Sum(x => x.TotalBetCount),
                SummaryStake = listBroadCasterOustanding.Sum(x => x.TotalStake),
                SummaryPayout = listBroadCasterOustanding.Sum(x => x.TotalPayout)
            };
        }

        private async Task<List<BroadCasterOutstandingDto>> GetBroadCasterOutstandings(IQueryable<Data.Entities.Ticket> ticketQuery, IBetKindRepository betKindRepos, GetBroadCasterOutstandingModel model)
        {
            var broadCasterOutsQuery = ticketQuery
                 .Join(betKindRepos.FindQuery(),
                 tk => new { tk.RegionId, tk.BetKindId },
                 bk => new { bk.RegionId, BetKindId = bk.Id },
                 (tk, bk) => new { bk.RegionId, bk.CategoryId, bk.Id, bk.Name, tk.Stake, tk.PlayerPayout, tk.ParentId, tk.ShowMore })
                 .GroupBy(joinedData => new { joinedData.RegionId, joinedData.CategoryId, joinedData.Id, joinedData.Name })
                 .Select(groupedData => new BroadCasterOutstandingDto
                 {
                     RegionId = groupedData.Key.RegionId,
                     CategoryId = groupedData.Key.CategoryId,
                     BetKindId = groupedData.Key.Id,
                     BetKindName = groupedData.Key.Name,
                     TotalStake = groupedData.Where(f => !f.ParentId.HasValue).Sum(x => x.Stake),
                     TotalPayout = groupedData.Where(f => !f.ParentId.HasValue).Sum(x => x.PlayerPayout),
                     TotalBetCount = groupedData.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue))
                 });

            if (model.SortType == SortType.Descending)
            {
                if (string.IsNullOrEmpty(model.SortName) || model.SortName.ToLower() == "betkind")
                {
                    broadCasterOutsQuery = broadCasterOutsQuery.OrderByDescending(x => x.RegionId).ThenByDescending(x => x.CategoryId).ThenByDescending(x => x.BetKindId);
                }
                else
                {
                    broadCasterOutsQuery = broadCasterOutsQuery.OrderByDescending(GetSortBroadCasterOutsProperty(model));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.SortName) || model.SortName.ToLower() == "betkind")
                {
                    broadCasterOutsQuery = broadCasterOutsQuery.OrderBy(x => x.RegionId).ThenBy(x => x.CategoryId).ThenBy(x => x.BetKindId);
                }
                else
                {
                    broadCasterOutsQuery = broadCasterOutsQuery.OrderBy(GetSortBroadCasterOutsProperty(model));
                }
            }
            var result = await broadCasterOutsQuery.ToListAsync();
            foreach (var item in result)
            {
                item.RegionName = EnumCategoryHelper.GetEnumRegionInformation((Region)item.RegionId)?.Name;
                item.CategoryName = EnumCategoryHelper.GetEnumCategoryInformation((Category)item.CategoryId)?.Name;
            }
            return result;
        }

        private Expression<Func<BroadCasterOutstandingDto, object>> GetSortBroadCasterOutsProperty(GetBroadCasterOutstandingModel model)
        {
            return model.SortName?.ToLower() switch
            {
                "totalbetcount" => broadCasterOuts => broadCasterOuts.TotalBetCount,
                "totalstake" => broadCasterOuts => broadCasterOuts.TotalStake,
                "totalpayout" => broadCasterOuts => broadCasterOuts.TotalPayout
            };
        }

        public async Task<GetBroadCasterWinlossSummaryResult> GetBroadCasterWinLossSummary(DateTime from, DateTime to, bool selectedDraft)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var ticketRepos = LotteryUow.GetRepository<ITicketRepository>();
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();
            var targetAgentId = ClientContext.Agent.ParentId == 0L
                                        ? ClientContext.Agent.AgentId
                                        : ClientContext.Agent.ParentId;
            var loginAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            var ticketStates = selectedDraft ? CommonHelper.AllTicketState() : CommonHelper.CompletedTicketWithoutRefundOrRejectState();
            var ticketQuery = ticketRepos.FindQueryBy(tk => ticketStates.Contains(tk.State) && tk.KickOffTime >= from.Date && tk.KickOffTime <= to.AddDays(1).AddTicks(-1));
            switch (loginAgent.RoleId)
            {
                case (int)Role.Company:
                    var companyTicketQuery = ticketQuery;
                    return await GetBroadCasterWinloss(companyTicketQuery, betKindRepos);
                case (int)Role.Supermaster:
                    var superMasterTicketQuery = ticketQuery.Where(x => x.SupermasterId == targetAgentId);
                    return await GetBroadCasterWinloss(superMasterTicketQuery, betKindRepos);
                case (int)Role.Master:
                    var masterTicketQuery = ticketQuery.Where(x => x.MasterId == targetAgentId);
                    return await GetBroadCasterWinloss(masterTicketQuery, betKindRepos);
                case (int)Role.Agent:
                    var agentTicketQuery = ticketQuery.Where(x => x.AgentId == targetAgentId);
                    return await GetBroadCasterWinloss(agentTicketQuery, betKindRepos);
                default:
                    return new GetBroadCasterWinlossSummaryResult();
            }
        }

        private async Task<GetBroadCasterWinlossSummaryResult> GetBroadCasterWinloss(IQueryable<Data.Entities.Ticket> ticketQuery, IBetKindRepository betKindRepos)
        {
            var broadCasterWinlossSummaries = await ticketQuery
                                   .Join(betKindRepos.FindQuery(),
                                   tk => new { tk.RegionId, tk.BetKindId },
                                   bk => new { bk.RegionId, BetKindId = bk.Id },
                                   (tk, bk) => new
                                   {
                                       bk.RegionId,
                                       bk.CategoryId,
                                       bk.Id,
                                       bk.Name,
                                       tk.Stake,
                                       tk.ShowMore,
                                       tk.ParentId,
                                       tk.PlayerPayout,
                                       tk.PlayerWinLoss,
                                       tk.DraftPlayerWinLoss,
                                       tk.AgentWinLoss,
                                       tk.AgentCommission,
                                       tk.DraftAgentWinLoss,
                                       tk.DraftAgentCommission,
                                       tk.MasterWinLoss,
                                       tk.MasterCommission,
                                       tk.DraftMasterWinLoss,
                                       tk.DraftMasterCommission,
                                       tk.SupermasterWinLoss,
                                       tk.SupermasterCommission,
                                       tk.DraftSupermasterWinLoss,
                                       tk.DraftSupermasterCommission,
                                       tk.CompanyWinLoss,
                                       tk.DraftCompanyWinLoss
                                   })
                                   .GroupBy(joinedData => new
                                   {
                                       joinedData.RegionId,
                                       joinedData.CategoryId,
                                       joinedData.Id,
                                       joinedData.Name
                                   })
                                   .Select(x => new BroadCasterWinlossSummaryDto
                                   {
                                       RegionId = x.Key.RegionId,
                                       CategoryId = x.Key.CategoryId,
                                       BetKindId = x.Key.Id,
                                       BetKindName = x.Key.Name,
                                       BetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       Point = x.Where(f => !f.ParentId.HasValue).Sum(s => s.Stake),
                                       Payout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.PlayerPayout),
                                       WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.PlayerWinLoss),
                                       DraftWinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.DraftPlayerWinLoss)
                                   })
                                   .OrderBy(x => x.RegionId)
                                   .ThenBy(x => x.CategoryId)
                                   .ThenBy(x => x.BetKindId)
                                   .ToListAsync();
            foreach (var item in broadCasterWinlossSummaries)
            {
                item.RegionName = EnumCategoryHelper.GetEnumRegionInformation((Region)item.RegionId)?.Name;
                item.CategoryName = EnumCategoryHelper.GetEnumCategoryInformation((Category)item.CategoryId)?.Name;
            }
            return new GetBroadCasterWinlossSummaryResult
            {
                BroadCasterWinlossSummaries = broadCasterWinlossSummaries,
                TotalBetCount = broadCasterWinlossSummaries.Sum(x => x.BetCount),
                TotalPoint = broadCasterWinlossSummaries.Sum(x => x.Point),
                TotalPayout = broadCasterWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = broadCasterWinlossSummaries.Sum(x => x.WinLose),
                TotalDraftWinLose = broadCasterWinlossSummaries.Sum(x => x.DraftWinLose)
            };
        }
    }
}

using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Announcement;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Agent
{
    public class NormalizePlayerService : LotteryBaseService<NormalizePlayerService>, INormalizePlayerService
    {
        private readonly IRedisCacheService _cacheService;
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;

        public NormalizePlayerService(ILogger<NormalizePlayerService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IRedisCacheService cacheService,
            IInMemoryUnitOfWork inMemoryUnitOfWork) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _cacheService = cacheService;
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
        }

        public async Task DeleteSupermaster(long supermasterId)
        {
            var betKindInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
            var betKinds = betKindInMemoryRepository.GetAll().ToList();

            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();

            var agents = await agentRepository.FindQueryBy(f => f.AgentId == supermasterId || f.SupermasterId == supermasterId || f.ParentId == supermasterId).ToListAsync();
            var agentIds = agents.Select(f => f.AgentId).ToList();

            var agentAuditRepository = LotteryUow.GetRepository<IAgentAuditRepository>();
            var agentAudits = await agentAuditRepository.FindQueryBy(f => agentIds.Contains(f.AgentId)).ToListAsync();

            var agentOddsRepository = LotteryUow.GetRepository<IAgentOddsRepository>();
            var agentOdds = await agentOddsRepository.FindQueryBy(f => agentIds.Contains(f.AgentId)).ToListAsync();

            var agentPositionTakingRepository = LotteryUow.GetRepository<IAgentPositionTakingRepository>();
            var agentPositionTakings = await agentPositionTakingRepository.FindQueryBy(f => agentIds.Contains(f.AgentId)).ToListAsync();

            var agentSessionRepository = LotteryUow.GetRepository<IAgentSessionRepository>();
            var agentSessions = await agentSessionRepository.FindQueryBy(f => agentIds.Contains(f.AgentId)).ToListAsync();

            var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
            var tickets = await ticketRepository.FindQueryBy(f => agentIds.Contains(f.SupermasterId)).ToListAsync();

            var agentAnnouncementRepository = LotteryUow.GetRepository<IAgentAnnouncementRepository>();
            var agentAnnouncements = await agentAnnouncementRepository.FindQueryBy(f => agentIds.Contains(f.AgentId)).ToListAsync();

            var players = await playerRepository.FindQueryBy(f => f.SupermasterId == supermasterId).ToListAsync();
            var playerIds = players.Select(f => f.PlayerId).ToList();

            var playerAnnouncementRepository = LotteryUow.GetRepository<IPlayerAnnouncementRepository>();
            var playerAnnouncements = await playerAnnouncementRepository.FindQueryBy(f => playerIds.Contains(f.PlayerId)).ToListAsync();

            var playerAuditRepository = LotteryUow.GetRepository<IPlayerAuditRepository>();
            var playerAudits = await playerAuditRepository.FindQueryBy(f => playerIds.Contains(f.PlayerId)).ToListAsync();

            var playerOddsRepository = LotteryUow.GetRepository<IPlayerOddsRepository>();
            var playerOdds = await playerOddsRepository.FindQueryBy(f => playerIds.Contains(f.PlayerId)).ToListAsync();

            var playerSessionRepository = LotteryUow.GetRepository<IPlayerSessionRepository>();
            var playerSessions = await playerSessionRepository.FindQueryBy(f => playerIds.Contains(f.PlayerId)).ToListAsync();

            foreach (var item in agents)
            {
                var sessionKey = item.RoleId.GetSessionKeyByRole(item.AgentId);
                await _cacheService.RemoveAsync(sessionKey, CachingConfigs.RedisConnectionForApp);
            }

            foreach (var item in players)
            {
                var sessionKey = Role.Player.ToInt().GetSessionKeyByRole(item.PlayerId);
                await _cacheService.RemoveAsync(sessionKey, CachingConfigs.RedisConnectionForApp);

                var givenCreditKey = item.PlayerId.GetPlayerGivenCredit();
                await _cacheService.HashDeleteFieldsAsync(givenCreditKey.MainKey, new List<string> { givenCreditKey.SubKey }, CachingConfigs.RedisConnectionForApp);

                foreach (var itemBetKind in betKinds)
                {
                    var betKindKey = item.PlayerId.GetMaxPerNumberPlayer(itemBetKind.Id);
                    await _cacheService.HashDeleteFieldsAsync(betKindKey.MainKey, new List<string> { betKindKey.SubKey }, CachingConfigs.RedisConnectionForApp);

                    var maxBetKey = item.PlayerId.GetMaxBetPlayer(itemBetKind.Id);
                    await _cacheService.HashDeleteFieldsAsync(maxBetKey.MainKey, new List<string> { maxBetKey.SubKey }, CachingConfigs.RedisConnectionForApp);

                    var minBetKey = item.PlayerId.GetMinBetPlayer(itemBetKind.Id);
                    await _cacheService.HashDeleteFieldsAsync(minBetKey.MainKey, new List<string> { minBetKey.SubKey }, CachingConfigs.RedisConnectionForApp);

                    var oddsKey = item.PlayerId.GetPlayerOddsByBetKind(itemBetKind.Id);
                    await _cacheService.HashDeleteFieldsAsync(oddsKey.MainKey, new List<string> { oddsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                }
            }

            foreach (var item in playerAnnouncements) playerAnnouncementRepository.Delete(item);
            foreach (var item in playerAudits) playerAuditRepository.Delete(item);
            foreach (var item in playerOdds) playerOddsRepository.Delete(item);
            foreach (var item in playerSessions) playerSessionRepository.Delete(item);
            foreach (var item in players) playerRepository.Delete(item);

            foreach (var item in agentAudits) agentAuditRepository.Delete(item);
            foreach (var item in agentOdds) agentOddsRepository.Delete(item);
            foreach (var item in agentPositionTakings) agentPositionTakingRepository.Delete(item);
            foreach (var item in agentSessions) agentSessionRepository.Delete(item);
            foreach (var item in tickets) ticketRepository.Delete(item);
            foreach (var item in agentAnnouncements) agentAnnouncementRepository.Delete(item);

            foreach (var item in agents) agentRepository.Delete(item);

            await LotteryUow.SaveChangesAsync();
        }

        public async Task NormalizePlayerBySupermaster(List<long> supermasterIds)
        {
            var agentOddsRepository = LotteryUow.GetRepository<IAgentOddsRepository>();
            var betSettings = await agentOddsRepository.FindQueryBy(f => f.AgentId == (ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId)).ToListAsync();

            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var agents = await agentRepository.FindQueryBy(f => f.ParentId == 0L && (supermasterIds.Contains(f.AgentId) || supermasterIds.Contains(f.SupermasterId))).ToListAsync();
            var agentIds = agents.Select(f => f.AgentId).ToList();

            var otherBetSettings = await agentOddsRepository.FindQueryBy(f => agentIds.Contains(f.AgentId)).ToListAsync();
            otherBetSettings.ForEach(f =>
            {
                var itemByBetKind = betSettings.FirstOrDefault(f1 => f1.BetKindId == f.BetKindId);
                if (itemByBetKind == null) return;

                f.MinBet = itemByBetKind.MinBet;
                agentOddsRepository.Update(f);
            });
            agentIds.ForEach(f =>
            {
                var agentBetSettings = otherBetSettings.Where(f1 => f1.AgentId == f).ToList();
                foreach (var item in betSettings)
                {
                    var agentBetSettingItem = agentBetSettings.FirstOrDefault(f1 => f1.BetKindId == item.BetKindId);
                    if (agentBetSettingItem != null) continue;

                    agentOddsRepository.Add(new Data.Entities.AgentOdd
                    {
                        AgentId = f,
                        BetKindId = item.BetKindId,
                        Buy = item.Buy,
                        MaxBet = item.MaxBet,
                        MaxBuy = item.MaxBuy,
                        MaxPerNumber = item.MaxPerNumber,
                        MinBet = item.MinBet,
                        MinBuy = item.MinBuy,
                        CreatedAt = item.CreatedAt,
                        CreatedBy = item.CreatedBy
                    });
                }
            });

            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var players = await playerRepository.FindQueryBy(f => supermasterIds.Contains(f.SupermasterId)).ToListAsync();
            var playerIds = players.Select(f => f.PlayerId).ToList();

            var playerOddsRepository = LotteryUow.GetRepository<IPlayerOddsRepository>();
            var playerBetSetting = await playerOddsRepository.FindQueryBy(f => playerIds.Contains(f.PlayerId)).ToListAsync();
            playerBetSetting.ForEach(f =>
            {
                var itemByBetKind = betSettings.FirstOrDefault(f1 => f1.BetKindId == f.BetKindId);
                if (itemByBetKind == null) return;

                f.MinBet = itemByBetKind.MinBet;
                playerOddsRepository.Update(f);
            });

            await LotteryUow.SaveChangesAsync();

            foreach (var item in playerBetSetting)
            {
                var minBetKey = item.PlayerId.GetMinBetPlayer(item.BetKindId);
                await _cacheService.HashSetAsync(minBetKey.MainKey, new Dictionary<string, string>
                {
                    { minBetKey.SubKey, item.MinBet.ToString() }
                }, minBetKey.TimeSpan == TimeSpan.Zero ? null : minBetKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
            }
        }
    }
}

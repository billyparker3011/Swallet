using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Helpers;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Agent
{
    public class NormalizePlayerService : LotteryBaseService<NormalizePlayerService>, INormalizePlayerService
    {
        private readonly IRedisCacheService _cacheService;

        public NormalizePlayerService(ILogger<NormalizePlayerService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IRedisCacheService cacheService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _cacheService = cacheService;
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

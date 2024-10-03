using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Options;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Authentication;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Authentication
{
    public class SessionService : LotteryBaseService<SessionService>, ISessionService
    {
        private const int _intervalCheckingStateInMinutes = 1;
        private readonly IRedisCacheService _cacheService;

        public SessionService(ILogger<SessionService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IRedisCacheService cacheService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _cacheService = cacheService;
        }

        public async Task CreateSession(SessionModel session)
        {
            var authenticationValidateOption = Configuration.GetSection(AuthenticationValidateOption.AppSettingName).Get<AuthenticationValidateOption>() ?? throw new UnhandledException("Cannot read AuthenticationValidate option.");
            var key = session.RoleId.GetSessionKeyByRole(session.TargetId);
            var entries = new Dictionary<string, string>
            {
                { nameof(SessionModel.Hash), session.Hash },
                { nameof(SessionModel.IpAddress), session.IpAddress },
                { nameof(SessionModel.UserAgent), session.UserAgent },
                { nameof(SessionModel.LatestDoingTime), session.LatestDoingTime.ToString(CachingConfigs.RedisFormatDateTime) },
                { nameof(SessionModel.RoleId), session.RoleId.ToString() },
                { nameof(SessionModel.TargetId), session.TargetId.ToString() },
                { nameof(SessionModel.LatestCheckingState), ClockService.GetUtcNow().ToString(CachingConfigs.RedisFormatDateTime) }
            };
            await _cacheService.HashSetAsync(key, entries, TimeSpan.FromSeconds(authenticationValidateOption.ExpiryInMinutes * 60), CachingConfigs.RedisConnectionForApp);
        }

        public async Task RemoveSession(int roleId, long targetId)
        {
            await _cacheService.RemoveAsync(roleId.GetSessionKeyByRole(targetId), CachingConfigs.RedisConnectionForApp);
        }

        public async Task<int> RecheckIn(bool isAgent = true)
        {
            var authenticationValidateOption = Configuration.GetSection(AuthenticationValidateOption.AppSettingName).Get<AuthenticationValidateOption>() ?? throw new UnhandledException("Cannot read AuthenticationValidate option.");

            //  Token
            var hash = isAgent ? ClientContext.Agent.Hash : ClientContext.Player.Hash;

            //  Session
            var roleId = isAgent ? ClientContext.Agent.RoleId : ClientContext.Player.RoleId;
            var targetId = isAgent ? ClientContext.Agent.AgentId : ClientContext.Player.PlayerId;
            var key = roleId.GetSessionKeyByRole(targetId);

            var dictHash = await _cacheService.HashGetFieldsAsync(key, new List<string> { nameof(SessionModel.Hash), nameof(SessionModel.LatestDoingTime), nameof(SessionModel.LatestCheckingState) }, CachingConfigs.RedisConnectionForApp);
            if (dictHash.Count == 0) return -1;
            if (!dictHash.TryGetValue(nameof(SessionModel.Hash), out string sessionHash) || string.IsNullOrEmpty(sessionHash)) return -1;
            if (!hash.Equals(sessionHash, StringComparison.Ordinal)) return -1;

            var entries = new Dictionary<string, string>
            {
                { nameof(SessionModel.LatestDoingTime), ClockService.GetUtcNow().ToString(CachingConfigs.RedisFormatDateTime) }
            };

            DateTime? latestCheckingState = null;
            if (dictHash.TryGetValue(nameof(SessionModel.LatestCheckingState), out string sLatestCheckingState))
                latestCheckingState = sLatestCheckingState.ToDateTime(CachingConfigs.RedisFormatDateTime);
            if (latestCheckingState == null) latestCheckingState = ClockService.GetUtcNow();
            if (latestCheckingState.Value.AddMinutes(_intervalCheckingStateInMinutes) < ClockService.GetUtcNow())
            {
                (var isLock, var isClosed) = await GetState(isAgent, targetId);
                if (isLock) return -2;
                if (isClosed) return -3;
                entries[nameof(SessionModel.LatestCheckingState)] = ClockService.GetUtcNow().ToString(CachingConfigs.RedisFormatDateTime);
            }
            await _cacheService.HashSetFieldsAsync(key, entries, TimeSpan.FromSeconds(authenticationValidateOption.ExpiryInMinutes * 60), CachingConfigs.RedisConnectionForApp);
            return 0;
        }

        private async Task<(bool, bool)> GetState(bool isAgent, long targetId)
        {
            return isAgent ? await GetAgentState(targetId) : await GetPlayerState(targetId);
        }

        private async Task<(bool, bool)> GetPlayerState(long targetId)
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var player = await playerRepository.FindByIdAsync(targetId);
            return player == null
                ? throw new NotFoundException()
                : ((bool, bool))(player.Lock, player.State == UserState.Closed.ToInt() || (player.ParentState.HasValue && player.ParentState.Value == UserState.Closed.ToInt()));
        }

        private async Task<(bool, bool)> GetAgentState(long targetId)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var agent = await agentRepository.FindByIdAsync(targetId);
            return agent == null
                ? throw new NotFoundException()
                : ((bool, bool))(agent.Lock, agent.State == UserState.Closed.ToInt() || (agent.ParentState.HasValue && agent.ParentState.Value == UserState.Closed.ToInt()));
        }
    }
}

using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Models;
using HnMicro.Framework.Services;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Auth;
using Lottery.Core.Models.Authentication;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Lottery.Core.Services.Authentication
{
    public class PlayerAuthenticationService : LotteryBaseService<PlayerAuthenticationService>, IPlayerAuthenticationService
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ISessionService _sessionService;

        public PlayerAuthenticationService(ILogger<PlayerAuthenticationService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IJwtTokenService jwtTokenService,
            ISessionService sessionService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _jwtTokenService = jwtTokenService;
            _sessionService = sessionService;
        }

        public async Task<JwtToken> Auth(AuthModel model)
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var player = await playerRepository.FindByUsernameAndPassword(model.Username.ToUpper(), model.Password.DecodePassword().Md5()) ?? throw new BadRequestException(ErrorCodeHelper.Auth.UserPasswordIsWrong);
            if (player.State == UserState.Closed.ToInt() || (player.ParentState != null && player.ParentState.Value == UserState.Closed.ToInt())) throw new BadRequestException(ErrorCodeHelper.Auth.UserClosed);
            if (player.Lock) throw new BadRequestException(ErrorCodeHelper.Auth.UserLocked);

            var isExpiredPassword = player.LatestChangePassword != null && player.LatestChangePassword.Value.IsExpiredDate();

            var roleId = Role.Player.ToInt();
            var playerId = player.PlayerId;
            var session = new SessionModel
            {
                RoleId = roleId,
                TargetId = playerId
            };
            await UpdateAuditAndSession(roleId, player, session);
            await _sessionService.CreateSession(session);

            var claims = new List<Claim>
            {
                new(ClaimConfigs.PlayerClaimConfig.PlayerId, player.PlayerId.ToString()),
                new(ClaimConfigs.Username, player.Username),
                new(ClaimConfigs.RoleId, roleId.ToString()),
                new(ClaimConfigs.FirstName, player.FirstName ?? string.Empty),
                new(ClaimConfigs.LastName, player.LastName ?? string.Empty),
                new(ClaimConfigs.NeedToChangePassword, isExpiredPassword.ToString().ToLower()),
                new(ClaimConfigs.SupermasterId, player.SupermasterId.ToString()),
                new(ClaimConfigs.MasterId, player.MasterId.ToString()),
                new(ClaimConfigs.AgentId, player.AgentId.ToString()),
                new(ClaimConfigs.Hash, session.Hash)
            };
            return _jwtTokenService.BuildToken(claims);
        }

        private async Task UpdateAuditAndSession(int roleId, Data.Entities.Player player, SessionModel session)
        {
            var clientInformation = ClientContext.GetClientInformation();
            if (clientInformation == null) return;

            var currentTime = ClockService.GetUtcNow();

            //  PlayerSession
            var hash = StringHelper.MaxHashLength.RandomString();
            var playerSessionRepository = LotteryUow.GetRepository<IPlayerSessionRepository>();
            var playerSession = player.PlayerSession;
            if (playerSession == null)
            {
                playerSession = new Data.Entities.PlayerSession
                {
                    PlayerId = player.PlayerId,
                    Hash = hash,
                    IpAddress = clientInformation.IpAddress,
                    LatestDoingTime = currentTime,
                    State = SessionState.Online.ToInt(),
                    UserAgent = clientInformation.UserAgent,
                    Platform = clientInformation.Platform
                };
                playerSessionRepository.Add(playerSession);
            }
            else
            {
                playerSession.Hash = hash;
                playerSession.IpAddress = clientInformation.IpAddress;
                playerSession.LatestDoingTime = currentTime;
                playerSession.State = SessionState.Online.ToInt();
                playerSession.UserAgent = clientInformation.UserAgent;
                playerSession.Platform = clientInformation.Platform;
                playerSessionRepository.Update(playerSession);
            }

            //  PlayerAudit
            LotteryUow.GetRepository<IPlayerAuditRepository>().Add(new Data.Entities.PlayerAudit
            {
                PlayerId = player.PlayerId,
                IpAddress = playerSession.IpAddress,
                UserAgent = playerSession.UserAgent,
                Headers = clientInformation.Header,
                Platform = clientInformation.Platform,
                CreatedAt = currentTime
            });

            //  Session
            if (session == null) session = new SessionModel { RoleId = roleId, TargetId = player.PlayerId };
            session.Hash = playerSession.Hash;
            session.IpAddress = playerSession.IpAddress;
            session.UserAgent = playerSession.UserAgent;
            session.Platform = playerSession.Platform;
            session.LatestDoingTime = playerSession.LatestDoingTime.Value;

            await LotteryUow.SaveChangesAsync();
        }
    }
}

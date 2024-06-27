using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Models;
using HnMicro.Framework.Services;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Auth;
using Lottery.Core.Models.Authentication;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Services.Audit;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Lottery.Core.Services.Authentication
{
    public class AgentAuthenticationService : LotteryBaseService<AgentAuthenticationService>, IAgentAuthenticationService
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ISessionService _sessionService;
        private readonly IAuditService _auditService;

        public AgentAuthenticationService(
            ILogger<AgentAuthenticationService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow,
            IJwtTokenService jwtTokenService,
            ISessionService sessionService,
            IAuditService auditService)
            : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _jwtTokenService = jwtTokenService;
            _sessionService = sessionService;
            _auditService = auditService;
        }

        public async Task<JwtToken> Auth(AuthModel model)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var user = await agentRepository.FindByUsername(model.Username.ToUpper()) ?? throw new BadRequestException(ErrorCodeHelper.Auth.UserPasswordIsWrong);
            if (!user.Password.Equals(model.Password.DecodePassword().Md5()))
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = (int)AuditType.Login,
                    Action = AuditDataHelper.Login.Action.Login,
                    DetailMessage = AuditDataHelper.Login.DetailMessage.FailByWrongUserPassword,
                    AgentUserName = user.Username,
                    AgentFirstName = user.FirstName,
                    AgentLastName = user.LastName,
                    SupermasterId = GetSupermasterId(user),
                    MasterId = GetMasterId(user),
                    AgentId = GetAgentId(user)
                });
                throw new BadRequestException(ErrorCodeHelper.Auth.UserPasswordIsWrong);
            }
            if (user.State == UserState.Closed.ToInt())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = (int)AuditType.Login,
                    Action = AuditDataHelper.Login.Action.Login,
                    AgentUserName = user.Username,
                    AgentFirstName = user.FirstName,
                    AgentLastName = user.LastName,
                    DetailMessage = string.Format(AuditDataHelper.Login.DetailMessage.SuccessButUserIsClosed, user.Username),
                    SupermasterId = GetSupermasterId(user),
                    MasterId = GetMasterId(user),
                    AgentId = GetAgentId(user)
                });
                throw new BadRequestException(ErrorCodeHelper.Auth.UserClosed);
            }

            var isExpiredPassword = user.LatestChangePassword != null && user.LatestChangePassword.Value.IsExpiredDate();
            var isExpiredSecurityCode = user.LatestChangeSecurityCode != null && user.LatestChangeSecurityCode.Value.IsExpiredDate();

            var roleId = user.RoleId;
            var agentId = user.AgentId;
            var session = new SessionModel
            {
                RoleId = roleId,
                TargetId = agentId
            };
            //  Update AgentAudit & AgentSession
            await UpdateAuditAndSession(user, session);
            await _sessionService.CreateSession(session);

            var claims = new List<Claim>
            {
                new(ClaimConfigs.AgentClaimConfig.AgentId, agentId.ToString()),
                new(ClaimConfigs.AgentClaimConfig.ParentId, user.ParentId.ToString()),
                new(ClaimConfigs.Username, user.Username),
                new(ClaimConfigs.RoleId, roleId.ToString()),
                new(ClaimConfigs.FirstName, user.FirstName ?? string.Empty),
                new(ClaimConfigs.LastName, user.LastName ?? string.Empty),
                new(ClaimConfigs.NeedToChangePassword, isExpiredPassword.ToString().ToLower()),
                new(ClaimConfigs.NeedToChangeSecurityCode, isExpiredSecurityCode.ToString().ToLower()),
                new(ClaimConfigs.SupermasterId, user.SupermasterId.ToString()),
                new(ClaimConfigs.MasterId, user.MasterId.ToString()),
                new(ClaimConfigs.Hash, session.Hash)
            };

            if (string.IsNullOrEmpty(user.Permissions))
            {
                claims.Add(new Claim(ClaimConfigs.AgentClaimConfig.Permissions, string.Empty));
            }
            else
            {
                var agentPermissions = user.Permissions.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in agentPermissions) claims.Add(new Claim(ClaimConfigs.AgentClaimConfig.Permissions, item));
            }

            await _auditService.SaveAuditData(new AuditParams
            {
                Type = (int)AuditType.Login,
                Action = AuditDataHelper.Login.Action.Login,
                AgentUserName = user.Username,
                AgentFirstName = user.FirstName,
                AgentLastName = user.LastName,
                DetailMessage = string.Format(AuditDataHelper.Login.DetailMessage.Success, user.Username),
                SupermasterId = GetSupermasterId(user),
                MasterId = GetMasterId(user),
                AgentId = GetAgentId(user)
            });

            return _jwtTokenService.BuildToken(claims);
        }

        private long GetAgentId(Data.Entities.Agent loginUser)
        {
            return 0; //TODO: Set this value when implement audit for player
        }

        private long GetMasterId(Data.Entities.Agent loginUser)
        {
            return loginUser.RoleId is (int)Role.Agent ? loginUser.MasterId : 0;
        }

        private long GetSupermasterId(Data.Entities.Agent loginUser)
        {
            return loginUser.RoleId is (int)Role.Master or (int)Role.Agent ? loginUser.SupermasterId : 0;
        }

        private async Task UpdateAuditAndSession(Data.Entities.Agent agent, SessionModel session)
        {
            var clientInformation = ClientContext.GetClientInformation();
            if (clientInformation == null) return;

            var currentTime = ClockService.GetUtcNow();

            //  AgentSessions
            var hash = StringHelper.MaxHashLength.RandomString();
            var agentSessionRepository = LotteryUow.GetRepository<IAgentSessionRepository>();
            var agentSession = agent.AgentSession;
            if (agentSession == null)
            {
                agentSession = new Data.Entities.AgentSession
                {
                    AgentId = agent.AgentId,
                    Hash = hash,
                    IpAddress = clientInformation.IpAddress,
                    LatestDoingTime = currentTime,
                    State = SessionState.Online.ToInt(),
                    UserAgent = clientInformation.UserAgent,
                    Platform = clientInformation.Platform
                };
                agentSessionRepository.Add(agentSession);
            }
            else
            {
                agentSession.Hash = hash;
                agentSession.IpAddress = clientInformation.IpAddress;
                agentSession.LatestDoingTime = currentTime;
                agentSession.State = SessionState.Online.ToInt();
                agentSession.UserAgent = clientInformation.UserAgent;
                agentSession.Platform = clientInformation.Platform;
                agentSessionRepository.Update(agentSession);
            }

            //  AgentAudit
            LotteryUow.GetRepository<IAgentAuditRepository>().Add(new Data.Entities.AgentAudit
            {
                AgentId = agent.AgentId,
                IpAddress = agentSession.IpAddress,
                UserAgent = agentSession.UserAgent,
                Platform = agentSession.Platform,
                Headers = clientInformation.Header,
                CreatedAt = currentTime
            });

            //  Session
            if (session == null) session = new SessionModel { RoleId = agent.RoleId, TargetId = agent.AgentId };
            session.Hash = hash;
            session.IpAddress = agentSession.IpAddress;
            session.UserAgent = agentSession.UserAgent;
            session.Platform = agentSession.Platform;
            session.LatestDoingTime = agentSession.LatestDoingTime.Value;

            await LotteryUow.SaveChangesAsync();
        }
    }
}

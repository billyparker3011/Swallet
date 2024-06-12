using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Auth;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Agent
{
    public class AgentSafeguardService : LotteryBaseService<AgentSafeguardService>, IAgentSafeguardService
    {
        public AgentSafeguardService(ILogger<AgentSafeguardService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task ResetLoginUserPassword(string password, string confirmPassword)
        {
            var loginAgentId = ClientContext.Agent.AgentId;
            await ResetPassword(new ResetPasswordModel
            {
                TargetId = loginAgentId,
                Password = password,
                ConfirmPassword = confirmPassword,
                IsSelfChange = true
            });
        }

        public async Task ResetLoginUserSercurityCode(string securityCode, string confirmSecurityCode)
        {
            var loginAgentId = ClientContext.Agent.AgentId;
            await ResetSercurityCode(new ResetSecurityCodeModel
            {
                TargetId = loginAgentId,
                SecurityCode = securityCode,
                ConfirmSecurityCode = confirmSecurityCode,
                IsSelfChange = true
            });
        }

        public async Task ResetPassword(ResetPasswordModel model)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var password = model.Password.DecodePassword();
            var confirmPassword = model.ConfirmPassword.DecodePassword();
            if (!password.IsStrongPassword()) throw new BadRequestException(ErrorCodeHelper.ChangeInfo.PasswordComplexityIsWeak);
            if (!password.Equals(confirmPassword, StringComparison.OrdinalIgnoreCase)) throw new BadRequestException(ErrorCodeHelper.ChangeInfo.NewPasswordDoesnotMatch);

            var agent = await agentRepository.FindByIdAsync(model.TargetId) ?? throw new NotFoundException();
            if (ClientContext.Agent.RoleId != Role.Company.ToInt() && !model.IsSelfChange)
            {
                if(agent.ParentId != 0L)
                {
                    if (agent.ParentId != ClientContext.Agent.AgentId) throw new NotFoundException();
                }
                else
                {
                    var agentId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
                    if (agentId != agent.SupermasterId && agentId != agent.MasterId) throw new NotFoundException();
                }
            }

            agent.Password = password.Md5();
            agent.UpdatedAt = ClockService.GetUtcNow();
            agent.UpdatedBy = ClientContext.Agent.AgentId;
            agent.LatestChangePassword = ClockService.GetUtcNow();
            await LotteryUow.SaveChangesAsync();
        }

        public async Task ResetSercurityCode(ResetSecurityCodeModel model)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();

            var securityCode = model.SecurityCode.DecodePassword();
            var confirmSecurityCode = model.ConfirmSecurityCode.DecodePassword();

            if (!securityCode.IsNumberOnly()) throw new BadRequestException(ErrorCodeHelper.ChangeInfo.WrongSecurityCodeFormat);
            if (!securityCode.Equals(confirmSecurityCode, StringComparison.OrdinalIgnoreCase)) throw new BadRequestException(ErrorCodeHelper.ChangeInfo.NewSecurityCodeDoesnotMatch);

            var agent = await agentRepository.FindByIdAsync(model.TargetId) ?? throw new NotFoundException();
            if (ClientContext.Agent.RoleId != Role.Company.ToInt() && !model.IsSelfChange)
            {
                if (agent.ParentId != 0L)
                {
                    if (agent.ParentId != ClientContext.Agent.AgentId) throw new NotFoundException();
                }
                else
                {
                    var agentId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
                    if (agentId != agent.SupermasterId && agentId != agent.MasterId) throw new NotFoundException();
                }
            }

            agent.SecurityCode = securityCode.Md5();
            agent.UpdatedAt = ClockService.GetUtcNow();
            agent.UpdatedBy = ClientContext.Agent.AgentId;
            agent.LatestChangeSecurityCode = ClockService.GetUtcNow();
            await LotteryUow.SaveChangesAsync();
        }
    }
}

using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Auth;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Player
{
    public class PlayerSafeguardService : LotteryBaseService<PlayerSafeguardService>, IPlayerSafeguardService
    {
        public PlayerSafeguardService(ILogger<PlayerSafeguardService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task ChangePassword(PlayerChangePasswordModel model)
        {
            var oldPassword = model.OldPassword.DecodePassword();
            var newPassword = model.NewPassword.DecodePassword();
            var confirmPassword = model.ConfirmPassword.DecodePassword();
            if (!oldPassword.IsStrongPassword() || !newPassword.IsStrongPassword() || !confirmPassword.IsStrongPassword()) throw new BadRequestException(ErrorCodeHelper.ChangeInfo.PasswordComplexityIsWeak);
            if (!newPassword.Equals(confirmPassword, StringComparison.OrdinalIgnoreCase)) throw new BadRequestException(ErrorCodeHelper.ChangeInfo.NewPasswordDoesnotMatch);
            if (newPassword.Equals(oldPassword, StringComparison.OrdinalIgnoreCase)) throw new BadRequestException(ErrorCodeHelper.ChangeInfo.NewPasswordMustNotBeEqualToOldPassword);

            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var player = await playerRepository.FindByIdAsync(ClientContext.Player.PlayerId);
            if (player == null) throw new NotFoundException();

            if (!player.Password.Equals(oldPassword.Md5(), StringComparison.OrdinalIgnoreCase)) throw new BadRequestException(ErrorCodeHelper.ChangeInfo.OldPasswordWrong);

            player.Password = newPassword.Md5();
            player.LatestChangePassword = ClockService.GetUtcNow();
            await LotteryUow.SaveChangesAsync();
        }

        public async Task ResetPassword(ResetPasswordModel model)
        {
            var password = model.Password.DecodePassword();
            var confirmPassword = model.ConfirmPassword.DecodePassword();
            if (!password.IsStrongPassword()) throw new BadRequestException(ErrorCodeHelper.ChangeInfo.PasswordComplexityIsWeak);
            if (!password.Equals(confirmPassword, StringComparison.OrdinalIgnoreCase)) throw new BadRequestException(ErrorCodeHelper.ChangeInfo.NewPasswordDoesnotMatch);

            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var player = await playerRepository.FindByIdAsync(model.TargetId) ?? throw new NotFoundException();
            if (password.Md5().Equals(player.Password, StringComparison.OrdinalIgnoreCase)) throw new BadRequestException(ErrorCodeHelper.ChangeInfo.NewPasswordMustNotBeEqualToOldPassword);
            if (ClientContext.Agent.RoleId != Role.Company.ToInt())
            {
                var agentId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
                if (agentId != player.SupermasterId && agentId != player.MasterId && agentId != player.AgentId) throw new NotFoundException();
            }

            player.Password = password.Md5();
            player.UpdatedAt = ClockService.GetUtcNow();
            player.UpdatedBy = ClientContext.Agent.AgentId;
            player.LatestChangePassword = ClockService.GetUtcNow();
            await LotteryUow.SaveChangesAsync();
        }
    }
}

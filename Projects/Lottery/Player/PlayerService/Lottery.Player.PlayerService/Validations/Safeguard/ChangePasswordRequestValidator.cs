using FluentValidation;
using Lottery.Player.PlayerService.Requests.Safeguard;

namespace Lottery.Player.PlayerService.Validations.Safeguard
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(f => f.OldPassword)
                .NotEmpty()
                .WithMessage(Core.Localizations.Messages.Auth.PasswordIsRequired);

            RuleFor(f => f.NewPassword)
                .NotEmpty()
                .WithMessage(Core.Localizations.Messages.Auth.PasswordIsRequired);

            RuleFor(f => f.ConfirmPassword)
                .NotEmpty()
                .WithMessage(Core.Localizations.Messages.Auth.PasswordIsRequired)
                .Equal(f => f.NewPassword)
                .WithMessage(Core.Localizations.Messages.Auth.PasswordIsIncorrect);
        }
    }
}

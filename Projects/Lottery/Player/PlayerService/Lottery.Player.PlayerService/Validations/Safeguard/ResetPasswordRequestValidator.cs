using FluentValidation;
using Lottery.Player.PlayerService.Requests.Safeguard;

namespace Lottery.Player.PlayerService.Validations.Safeguard
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(f => f.Password)
                .NotEmpty()
                .WithMessage(Core.Localizations.Messages.Auth.PasswordIsRequired);

            RuleFor(f => f.ConfirmPassword)
                .NotEmpty()
                .WithMessage(Core.Localizations.Messages.Auth.PasswordIsRequired)
                .Equal(f => f.Password)
                .WithMessage(Core.Localizations.Messages.Auth.PasswordIsIncorrect);
        }
    }
}

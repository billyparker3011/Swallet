using FluentValidation;
using Lottery.Agent.AgentService.Requests.Safeguard;

namespace Lottery.Agent.AgentService.Validations.Safeguard
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

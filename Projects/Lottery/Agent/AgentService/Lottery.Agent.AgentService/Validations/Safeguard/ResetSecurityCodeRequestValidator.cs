using FluentValidation;
using Lottery.Agent.AgentService.Requests.Safeguard;

namespace Lottery.Agent.AgentService.Validations.Safeguard
{
    public class ResetSecurityCodeRequestValidator : AbstractValidator<ResetSecurityCodeRequest>
    {
        public ResetSecurityCodeRequestValidator()
        {
            RuleFor(f => f.SecurityCode)
                .NotEmpty()
                .WithMessage(Core.Localizations.Messages.Auth.PasswordIsRequired);

            RuleFor(f => f.ConfirmSecurityCode)
                .NotEmpty()
                .WithMessage(Core.Localizations.Messages.Auth.SecurityCodeIsRequired)
                .Equal(f => f.SecurityCode)
                .WithMessage(Core.Localizations.Messages.Auth.SecurityCodeIsIncorrect);
        }
    }
}

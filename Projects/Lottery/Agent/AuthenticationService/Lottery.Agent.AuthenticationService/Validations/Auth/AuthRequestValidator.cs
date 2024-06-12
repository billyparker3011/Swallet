using FluentValidation;
using Lottery.Agent.AuthenticationService.Requests.Auth;
using Lottery.Core.Localizations;

namespace Lottery.Agent.AuthenticationService.Validations.Auth
{
    public class AuthRequestValidator : AbstractValidator<AuthRequest>
    {
        public AuthRequestValidator()
        {
            RuleFor(f => f.Username)
                .NotEmpty()
                .WithMessage(Messages.Auth.UserNameIsRequired);

            RuleFor(f => f.Password)
                .NotEmpty()
                .WithMessage(Messages.Auth.PasswordIsRequired);
        }
    }
}

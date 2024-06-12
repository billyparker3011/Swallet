using FluentValidation;
using Lottery.Player.AuthenticationService.Requests.Auth;

namespace Lottery.Player.AuthenticationService.Validations.Auth
{
    public class AuthRequestValidation : AbstractValidator<AuthRequest>
    {
        public AuthRequestValidation()
        {
            RuleFor(f => f.Username)
                .NotNull().WithMessage(Core.Localizations.Messages.Auth.UserNameIsBlank);

            RuleFor(f => f.Password)
                .NotNull().WithMessage(Core.Localizations.Messages.Auth.PasswordIsBlank);
        }
    }
}

using FluentValidation;
using SWallet.Core.Consts;
using SWallet.ManagerService.Requests.Auth;

namespace SWallet.ManagerService.Validations.Auth
{
    public class AuthRequestValidator : AbstractValidator<AuthRequest>
    {
        public AuthRequestValidator()
        {
            RuleFor(f => f.Username)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.UserNameIsRequired);

            RuleFor(f => f.Password)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.PasswordIsRequired);
        }
    }
}

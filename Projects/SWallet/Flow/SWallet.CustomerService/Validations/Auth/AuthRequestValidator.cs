using FluentValidation;
using SWallet.Core.Consts;
using SWallet.CustomerService.Requests.Auth;

namespace SWallet.CustomerService.Validations.Auth
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

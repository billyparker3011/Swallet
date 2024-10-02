using FluentValidation;
using HnMicro.Core.Helpers;
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
                .WithMessage(CommonMessageConsts.PasswordIsRequired)
                .Custom((password, context) =>
                {
                    var decodePassword = password.DecodePassword();
                    if (decodePassword.Length < CommonMessageConsts.MinLengthOfPassword)
                    {
                        context.AddFailure(CommonMessageConsts.LengthOfPasswordAtLeast);
                        return;
                    }
                    if (!password.IsStrongPassword())
                    {
                        context.AddFailure(CommonMessageConsts.PasswordIsTooWeak);
                        return;
                    }
                });
        }
    }
}

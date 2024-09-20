using FluentValidation;
using HnMicro.Core.Helpers;
using SWallet.CustomerService.Requests.Customer;

namespace SWallet.CustomerService.Validations.Customer
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(f => f.OldPassword)
                .NotEmpty()
                .WithMessage(Core.Consts.CommonMessageConsts.PasswordIsRequired);

            RuleFor(f => f.NewPassword)
                .NotEmpty()
                .WithMessage(Core.Consts.CommonMessageConsts.PasswordIsRequired)
                .NotEqual(f => f.OldPassword)
                .WithMessage(Core.Consts.CommonMessageConsts.NewPasswordCanNotBeOldPassword)
                .Custom((password, context) =>
                {
                    if (!password.IsStrongPassword())
                    {
                        context.AddFailure(Core.Consts.CommonMessageConsts.PasswordIsTooWeak);
                    }
                });

            RuleFor(f => f.ConfirmPassword)
                .NotEmpty()
                .WithMessage(Core.Consts.CommonMessageConsts.PasswordIsRequired)
                .Equal(f => f.NewPassword)
                .WithMessage(Core.Consts.CommonMessageConsts.ConfirmPasswordDoesNotMatch);
        }
    }
}

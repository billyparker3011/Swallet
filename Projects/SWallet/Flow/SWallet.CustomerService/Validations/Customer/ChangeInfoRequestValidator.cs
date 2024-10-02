using FluentValidation;
using SWallet.CustomerService.Requests.Customer;

namespace SWallet.CustomerService.Validations.Customer
{
    public class ChangeInfoRequestValidator : AbstractValidator<ChangeInfoRequest>
    {
        public ChangeInfoRequestValidator()
        {
            RuleFor(f => f.FirstName)
                .NotEmpty()
                .WithMessage(Core.Consts.CommonMessageConsts.PasswordIsRequired);

            RuleFor(f => f.LastName)
                .NotEmpty()
                .WithMessage(Core.Consts.CommonMessageConsts.PasswordIsRequired);
        }
    }
}

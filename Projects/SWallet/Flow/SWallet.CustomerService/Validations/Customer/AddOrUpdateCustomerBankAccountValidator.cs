using FluentValidation;
using SWallet.Core.Consts;
using SWallet.CustomerService.Requests.Customer;

namespace SWallet.CustomerService.Validations.Customer
{
    public class AddOrUpdateCustomerBankAccountValidator : AbstractValidator<AddOrUpdateCustomerBankAccountRequest>
    {
        public AddOrUpdateCustomerBankAccountValidator()
        {
            RuleFor(f => f.NumberAccount)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.NumberAccountIsRequired)
                .Matches(@"[A-Z\d]")
                .WithMessage(CommonMessageConsts.NumberAccountDoesNotContainSpecialCharacters)
;
            RuleFor(f => f.CardHolder)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.CardHolderIsRequired);

            RuleFor(f => f.BankId)
                .GreaterThan(0)
                .WithMessage(CommonMessageConsts.BankIsRequire);
        }
    }
}

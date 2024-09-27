using FluentValidation;
using SWallet.Core.Consts;
using SWallet.CustomerService.Requests.Payment;

namespace SWallet.CustomerService.Validations.Payment
{
    public class DepositRequestValidator : AbstractValidator<DepositRequest>
    {
        public DepositRequestValidator()
        {
            RuleFor(f => f.PaymentMethodCode)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.PaymentMethodCodeIsRequired);

            RuleFor(f => f.BankId)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.BankIsRequired);

            RuleFor(f => f.BankAccountId)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.BankAccountIsRequired);

            RuleFor(f => f.CustomerBankAccountId)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.CustomerBankAccountIsRequired);

            RuleFor(f => f.Amount)
                .GreaterThan(0m)
                .WithMessage(CommonMessageConsts.AmountIsGreaterThanZero);

            RuleFor(f => f.Content)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.PaymentContentIsRequired);
        }
    }
}

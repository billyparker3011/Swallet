using FluentValidation;
using SWallet.Core.Consts;
using SWallet.CustomerService.Requests.Payment;

namespace SWallet.CustomerService.Validations.Payment
{
    public class WithdrawRequestValidator : AbstractValidator<WithdrawRequest>
    {
        public WithdrawRequestValidator()
        {
            RuleFor(f => f.CustomerBankAccountId)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.CustomerBankAccountIsRequired);

            RuleFor(f => f.PaymentMethodCode)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.PaymentMethodCodeIsRequired);

            RuleFor(f => f.Amount)
                .GreaterThan(0m)
                .WithMessage(CommonMessageConsts.AmountIsGreaterThanZero);
        }
    }
}

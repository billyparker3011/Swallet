using FluentValidation;
using SWallet.Core.Consts;
using SWallet.ManagerService.Requests.Payment;

namespace SWallet.ManagerService.Validations.Payment
{
    public class CreatePaymentMethodRequestValidator : AbstractValidator<CreatePaymentMethodRequest>
    {
        public CreatePaymentMethodRequestValidator()
        {
            RuleFor(f => f.Name)
                .NotNull()
                .WithMessage(CommonMessageConsts.PaymentMethodNameCannotBeNull);

            RuleFor(f => f.Code)
                .NotNull()
                .WithMessage(CommonMessageConsts.PaymentMethodCodeCannotBeNull);
        }
    }
}

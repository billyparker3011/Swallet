using FluentValidation;
using SWallet.Core.Consts;
using SWallet.ManagerService.Requests;

namespace SWallet.ManagerService.Validations.Transaction
{
    public class CompletedTransactionRequestValidator : AbstractValidator<CompletedTransactionRequest>
    {
        public CompletedTransactionRequestValidator()
        {
            RuleFor(f => f.Amount)
                .NotNull()
                .WithMessage(CommonMessageConsts.TransactionAmount)
                .GreaterThan(0m)
                .WithMessage(CommonMessageConsts.TransactionAmountMustBeGreaterThanZero);
        }
    }
}

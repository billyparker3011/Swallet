using FluentValidation;
using SWallet.Core.Consts;
using SWallet.ManagerService.Requests;

namespace SWallet.ManagerService.Validations.Bank
{
    public class CreateBankAccountRequestValidator : AbstractValidator<CreateBankAccountRequest>
    {
        public CreateBankAccountRequestValidator()
        {
            RuleFor(x => x.BankId)
                .NotNull()
                .WithMessage(CommonMessageConsts.BankIdIsRequired);
            RuleFor(x => x.NumberAccount)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.NumberAccountIsRequired);
            RuleFor(x => x.CardHolder)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.CardHolderIsRequired);
        }
    }
}

using FluentValidation;
using SWallet.Core.Consts;
using SWallet.ManagerService.Requests;

namespace SWallet.ManagerService.Validations.Bank
{
    public class CreateBankRequestValidator : AbstractValidator<CreateBankRequest>
    {
        public CreateBankRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.BankNameIsRequired);
            RuleFor(x => x.Icon)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.BankIconIsRequired);
        }
    }
}

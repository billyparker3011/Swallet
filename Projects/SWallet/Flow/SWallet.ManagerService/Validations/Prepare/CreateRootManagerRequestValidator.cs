using FluentValidation;
using SWallet.ManagerService.Requests.Prepare;

namespace SWallet.ManagerService.Validations.Prepare
{
    public class CreateRootManagerRequestValidator : AbstractValidator<CreateRootManagerRequest>
    {
        public CreateRootManagerRequestValidator()
        {
            //RuleFor(f => f.Token)
            //    .NotEmpty()
            //    .WithMessage(CommonMessageConsts.PrepareTokenIsRequired);
        }
    }
}

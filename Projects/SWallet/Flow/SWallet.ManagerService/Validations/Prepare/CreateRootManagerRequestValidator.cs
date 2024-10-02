using FluentValidation;
using SWallet.Core.Consts;
using SWallet.ManagerService.Requests.Prepare;

namespace SWallet.ManagerService.Validations.Prepare
{
    public class CreateRootManagerRequestValidator : AbstractValidator<CreateRootManagerRequest>
    {
        public CreateRootManagerRequestValidator()
        {
            RuleFor(f => f.LengthOfUsername)
                .GreaterThanOrEqualTo(OtherConsts.MinLengthOfUsername)
                .WithMessage(CommonMessageConsts.LengthOfUserNameAtLeast)
                .LessThanOrEqualTo(OtherConsts.MaxLengthOfUsername)
                .WithMessage(CommonMessageConsts.LengthOfUserNameExceed);

            RuleFor(f => f.LengthOfPassword)
                .GreaterThanOrEqualTo(OtherConsts.MinLengthOfPassword)
                .WithMessage(CommonMessageConsts.LengthOfPasswordAtLeast);
        }
    }
}

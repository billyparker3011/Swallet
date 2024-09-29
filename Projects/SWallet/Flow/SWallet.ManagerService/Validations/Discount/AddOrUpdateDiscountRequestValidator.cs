using FluentValidation;
using SWallet.Core.Consts;
using SWallet.ManagerService.Requests.Discount;

namespace SWallet.ManagerService.Validations.Discount
{
    public class AddOrUpdateDiscountRequestValidator : AbstractValidator<AddOrUpdateDiscountRequest>
    {
        public AddOrUpdateDiscountRequestValidator()
        {
            RuleFor(f => f.Name)
                .NotNull()
                .WithMessage(CommonMessageConsts.DiscountNameCanNotBeNull);

            RuleFor(f => f.Description)
                .NotNull()
                .WithMessage(CommonMessageConsts.DiscountDescriptionCanNotBeNull);

            RuleFor(f => f)
                .Custom((request, context) =>
                {
                    if (request.IsStatic && (request.Setting == null || request.Setting.Deposit == null))
                    {
                        context.AddFailure(CommonMessageConsts.DiscountDescriptionCanNotBeNull);
                        return;
                    }

                    if (request.StartedDate.HasValue && request.EndedDate.HasValue && request.StartedDate.Value > request.EndedDate.Value)
                    {
                        context.AddFailure(CommonMessageConsts.DiscountAppliedTimeIsWrong);
                        return;
                    }
                });
        }
    }
}

using FluentValidation;
using SWallet.Core.Consts;
using SWallet.ManagerService.Requests.Setting;

namespace SWallet.ManagerService.Validations.Setting
{
    public class UpdateSettingRequestValidator : AbstractValidator<UpdateSettingRequest>
    {
        public UpdateSettingRequestValidator()
        {
            RuleFor(f => f.Mask)
                .NotNull()
                .WithMessage(CommonMessageConsts.SettingMaskCanNotBeNull);

            RuleFor(f => f.Mask.NumberOfMaskCharacters)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.SettingNumberOfMaskCharactersMustBeGreaterThanZero);

            RuleFor(f => f.Mask.MaskCharacter)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.SettingMaskCharacterCanNotBeNull)
                .Custom((maskChar, context) =>
                {
                    if (maskChar.Length != 1)
                    {
                        context.AddFailure(CommonMessageConsts.SettingMaskCharacterCanNotBeNull);
                    }
                });

            RuleFor(f => f.Currency)
                .NotNull()
                .WithMessage(CommonMessageConsts.SettingCurrencyCanNotBeNull);

            RuleFor(f => f.Currency.CurrencySymbol)
                .NotNull()
                .WithMessage(CommonMessageConsts.SettingCurrencySymbolCanNotBeNull);

            RuleFor(f => f.PaymentPartner)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.SettingPaymentPartnerCanNotBeNull);
        }
    }
}

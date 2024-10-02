using FluentValidation;
using HnMicro.Core.Helpers;
using SWallet.Core.Consts;
using SWallet.CustomerService.Requests.Customer;

namespace SWallet.CustomerService.Validations.Customer
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(f => f.FirstName)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.FirstNameIsRequired);

            RuleFor(f => f.LastName)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.LastNameIsRequired);

            RuleFor(f => f.Email)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.EmailIsRequired)
                .EmailAddress()
                .WithMessage(CommonMessageConsts.EmailIsNotValid);

            RuleFor(f => f.Username)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.UserNameIsRequired)
                .Matches(@"[A-Z\d]")
                .WithMessage(CommonMessageConsts.UserNameDoesNotContainSpecialCharacters)
                .MinimumLength(CommonMessageConsts.MinLengthOfUserName)
                .MaximumLength(CommonMessageConsts.MaxLengthOfUserName);

            RuleFor(f => f.Password)
                .NotEmpty()
                .WithMessage(CommonMessageConsts.PasswordIsRequired)
                .Custom((password, context) =>
                {
                    var decodePassword = password.DecodePassword();
                    if (decodePassword.Length < CommonMessageConsts.MinLengthOfPassword)
                    {
                        context.AddFailure(CommonMessageConsts.LengthOfPasswordAtLeast);
                        return;
                    }
                    if (!password.IsStrongPassword())
                    {
                        context.AddFailure(CommonMessageConsts.PasswordIsTooWeak);
                        return;
                    }
                });

            RuleFor(f => f.Accepted)
                .Equal(true)
                .WithMessage(CommonMessageConsts.DoneAcceptTermAndConditionAndPolicy);
        }
    }
}

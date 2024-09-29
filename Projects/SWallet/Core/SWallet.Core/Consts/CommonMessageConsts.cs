namespace SWallet.Core.Consts
{
    public static class CommonMessageConsts
    {
        public const string RoleHasNotBeenInitialYet = "Role has not been initial yet.";
        public const string PaymentPartnerHasNotBeenInitialYet = "Payment Partner has not been initial yet.";
        public const string PaymentPartnerDoesNotExist = "Payment Partner does not exist.";

        public const string PaymentMethodCodeIsRequired = "Payment Method Code is required.";
        public const string CustomerBankAccountIsRequired = "Customer Bank Account is required.";
        public const string BankAccountIsRequired = "Bank Account is required.";
        public const string AmountIsGreaterThanZero = "Amount must be greater than zero.";
        public const string PaymentContentIsRequired = "Payment Content is required.";

        public const string UserNameIsRequired = "Username is required.";
        public const int MinLengthOfUserName = 15;
        public const int MaxLengthOfUserName = 250;
        public const string LengthOfUserNameAtLeast = "Length of Username is at least 15 characters.";
        public const string LengthOfUserNameExceed = "Length of Username is exceed 250 characters.";
        public const string UserNameDoesNotContainSpecialCharacters = "Username does not contain special characters";

        public const string PasswordIsRequired = "Password is required.";
        public const string LengthOfPasswordAtLeast = "Length of Password is at least 12 characters.";
        public const int MinLengthOfPassword = 12;
        public const string PasswordIsTooWeak = "Password is too weak.";

        #region Bank
        public const string BankNameIsRequired = "Bank Name is required.";
        public const string BankIconIsRequired = "Bank Icon is required.";
        public const string BankIdIsRequired = "BankId is required.";
        public const string NumberAccountIsRequired = "Number Account is required.";
        public const string NumberAccountDoesNotContainSpecialCharacters = "Number Account doesn't contain space and special characters.";
        public const string CardHolderIsRequired = "Card Holder is required.";
        public const string BankIsRequired = "Bank is required.";
        #endregion

        #region Customer
        public const string FirstNameIsRequired = "Firstname is required.";
        public const string LastNameIsRequired = "Lastname is required.";
        public const string EmailIsRequired = "Email is required.";
        public const string EmailIsNotValid = "Email is not a valid email address.";
        public const string EmailWasUsed = "Email was used. Please choose another email.";
        public const string DoneAcceptTermAndConditionAndPolicy = "You must accept Terms & Conditions and Privacy Policy.";
        public const string UsernameWasUsed = "Username was used. Please choose another username.";
        public const string CouldNotFindAgentPromoCode = "Could not find Agent's promo code.";
        public const string ConfirmPasswordDoesNotMatch = "Password doesn't match.";
        public const string NewPasswordCanNotBeOldPassword = "New Password cann't be equal to Old Password.";
        public const string OldPasswordDoesNotMatch = "Old Password doesn't match.";
        #endregion

        #region Setting
        public const string SettingMaskCanNotBeNull = "Mask cannot be NULL.";
        public const string SettingNumberOfMaskCharactersMustBeGreaterThanZero = "No.of mask must be greater than zero.";
        public const string SettingMaskCharacterCanNotBeNull = "Mask character cannot be NULL.";
        public const string SettingMaskCharacterContainsOnlyOneCharacter = "Mask character contains only one character.";

        public const string SettingCurrencyCanNotBeNull = "Currency cannot be NULL.";
        public const string SettingCurrencySymbolCanNotBeNull = "Currency symbol cannot be NULL.";

        public const string SettingPaymentPartnerCanNotBeNull = "Payment partner cannot be NULL.";

        public const string PaymentMethodNameCannotBeNull = "Payment method name cannot be NULL.";
        public const string PaymentMethodCodeCannotBeNull = "Payment method code cannot be NULL.";
        public const string PaymentMethodCodeExists = "Payment method code has been used.";
        #endregion
    }
}

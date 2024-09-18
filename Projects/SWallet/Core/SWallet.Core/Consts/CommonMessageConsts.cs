﻿namespace SWallet.Core.Consts
{
    public static class CommonMessageConsts
    {
        public const string RoleHasNotBeenInitialYet = "Role has not been initial yet.";

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
        public const string CardHolderIsRequired = "Card Holder is required.";
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
        #endregion
    }
}
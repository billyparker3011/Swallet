namespace SWallet.Core.Consts
{
    public static class CommonMessageConsts
    {
        public const string UserNameIsRequired = "Username is required.";
        public const string LengthOfUserNameAtLeast = "Length of Username is at least 15 characters.";
        public const string LengthOfUserNameExceed = "Length of Username is exceed 250 characters.";

        public const string PasswordIsRequired = "Password is required.";
        public const string LengthOfPasswordAtLeast = "Length of Password is at least 12 characters.";

        #region Bank
        public const string BankNameIsRequired = "Bank Name is required.";
        public const string BankIconIsRequired = "Bank Icon is required.";
        #endregion
    }
}

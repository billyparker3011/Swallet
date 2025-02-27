﻿namespace SWallet.Core.Helpers
{
    public static class ErrorCodeHelper
    {
        public static class Auth
        {
            public const int UsernameOrPasswordIsWrong = -1;
            public const int UserClosed = -2;
            public const int UserLocked = -3;
        }

        public static class Manager
        {
            public const int UsernameIsExists = -1;
        }

        public static class ChangeInfo
        {
            public const int PasswordComplexityIsWeak = -1;
        }
    }
}

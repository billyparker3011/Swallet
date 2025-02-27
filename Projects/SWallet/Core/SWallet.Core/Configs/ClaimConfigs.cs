﻿namespace SWallet.Core.Configs
{
    public static class ClaimConfigs
    {
        public const string Username = "Username";
        public const string RoleId = "RoleId";
        public const string FullName = "FullName";

        public const string NeedToChangePassword = "NeedToChangePassword";
        public const string NeedToChangeSecurityCode = "NeedToChangeSecurityCode";

        public const string SupermasterId = "SupermasterId";
        public const string MasterId = "MasterId";
        public const string AgentId = "AgentId";
        public const string Hash = "Hash";

        public static class ManagerClaimConfig
        {
            public const string ManagerId = "ManagerId";
            public const string ParentId = "ParentId";
            public const string ManagerRole = "ManagerRole";
            public const string Permissions = "Permissions";
        }

        public static class CustomerClaimConfig
        {
            public const string CustomerId = "CustomerId";
            public const string FirstName = "FirstName";
            public const string LastName = "LastName";
            public const string Email = "Email";
            public const string Telegram = "Telegram";
            public const string IsAffiliate = "IsAffiliate";
        }
    }
}

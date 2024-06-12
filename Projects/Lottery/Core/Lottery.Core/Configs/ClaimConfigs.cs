namespace Lottery.Core.Configs
{
    public static class ClaimConfigs
    {
        public const string RoleId = "RoleId";
        public const string Username = "Username";
        public const string FirstName = "FirstName";
        public const string LastName = "LastName";
        public const string NeedToChangePassword = "NeedToChangePassword";
        public const string NeedToChangeSecurityCode = "NeedToChangeSecurityCode";
        public const string SupermasterId = "SupermasterId";
        public const string MasterId = "MasterId";
        public const string AgentId = "AgentId";
        public const string Hash = "Hash";

        public static class AgentClaimConfig
        {
            public const string AgentId = "AgentId";
            public const string ParentId = "ParentId";
            public const string Permissions = "Permissions";
        }

        public static class PlayerClaimConfig
        {
            public const string PlayerId = "PlayerId";
        }
    }
}

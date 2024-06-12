namespace Lottery.Core.Helpers
{
    public static class AuditDataHelper
    {
        public const string DefaultCountry = "Viet Nam";
        public static class Login
        {
            public static class Action
            {
                public const string Login = "Login via Agent's Site";
            }
            public static class DetailMessage
            {
                public const string Success = "Agent: {0} - [Before Login] Result: Success";
                public const string SuccessButUserIsClosed = "Agent: {0} - [Before Login] Result: Success but user is closed";
                public const string FailByWrongUserPassword = "[Before Login] Result: Failed because user and password is wrong";
            }
        }

        public static class Credit
        {
            public static class Action
            {
                public const string ActionSetCreditWhenUserCreated = "Set Credit when Agent's created";
                public const string ActionUpdateGivenCredit = "Update Given Credit";
                public const string ActionUpdateAgentCredit = "Update Agent's Credit";
            }
            public static class DetailMessage
            {
                public const string DetailSetCreditWhenUserCreated = "UserName = {0}, MaxCredit = {1}, SubAccName = {2}";
                public const string DetailUpdateGivenCredit = "Update Given Credit for Account {0}";
                public const string DetailUpdateGivenCreditWithMemberMaxCredit = "Update Given Credit for Account {0} - Member's Max Credit (from {1} to {2})";
                public const string DetailUpdateAgentCredit = "Update credit for agent account {0}";
            }
        }
    }
}

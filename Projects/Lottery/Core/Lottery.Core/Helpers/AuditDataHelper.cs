using Lottery.Core.Enums;

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
                public const string ActionUpdatePlayerCredit = "Update Player's Credit";
            }
            public static class DetailMessage
            {
                public const string DetailSetCreditWhenUserCreated = "UserName = {0}, MaxCredit = {1}, SubAccName = {2}";
                public const string DetailUpdateGivenCredit = "Update Given Credit for Account {0}";
                public const string DetailUpdateGivenCreditWithMemberMaxCredit = "Update Given Credit for Account {0} - Member's Max Credit (from {1} to {2})";
                public const string DetailUpdateAgentCredit = "Update credit for agent account {0}";
                public const string DetailUpdatePlayerCredit = "Update credit for player account {0}";
            }
        }
        public static class State
        {
            public static class Action
            {
                public const string ActionUpdateState = "Change State";
            }
            public static class DetailMessage
            {
                public const string DetailUpdateState = "Change state of {0} from {1} to {2}";
            }
        }
        public static class Setting
        {
            public const string MinBetTitle = "Min Bet";
            public const string MaxBetTitle = "Max Bet";
            public const string MaxPerNumberTitle = "Max Per Number";
            public const string BuyTitle = "Buy";
            public const string PositionTakingTitle = "Position Taking";
            public const string CockFightMainLimitAmountPerFight = "Main Limit Amount Per Fight";
            public const string CockFightDrawLimitAmountPerFight = "Draw Limit Amount Per Fight";
            public const string CockFightLimitNumTicketPerFight = "Limit Number Ticket Per Fight";
            public static class Action
            {
                public const string ActionUpdateBetSetting = "Change Bet Setting";
                public const string ActionUpdatePositionTaking = "Change Position Taking";
                public const string ActionUpdateBuySetting = "Change Buy Setting";
            }
        }

        public static long GetAuditMasterId(Data.Entities.Agent targetUser)
        {
            return targetUser.RoleId is (int)Role.Agent ? targetUser.MasterId : 0;
        }

        public static long GetAuditSupermasterId(Data.Entities.Agent targetUser)
        {
            return targetUser.RoleId is (int)Role.Master or (int)Role.Agent ? targetUser.SupermasterId : 0;
        }
    }
}

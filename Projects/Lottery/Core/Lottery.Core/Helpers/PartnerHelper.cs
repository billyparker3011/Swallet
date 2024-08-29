using System.Security.Cryptography.Xml;

namespace Lottery.Core.Helpers
{
    public static class PartnerHelper
    {
        public static class CasinoPartnerKey
        {
            public static string CasinoBookieSettingKey = "CABSKey";
        }

        public static class CasinoReponseCode
        {
            public static int Success = 0;
            public static int Invalid_Operator_ID = 10000;
            public static int Invalid_Signature = 10001;
            public static int Player_account_does_not_exist = 10003;
            public static int Player_account_is_disabled_or_not_allowed_to_log_in = 10005;
            public static int Transaction_not_existed = 10006;
            public static int Invalid_status = 10007;
            public static int Player_is_offline_or_logged_out = 10008;
            public static int Prohibit_to_bet = 10100;
            public static int Credit_is_not_enough = 10101;
            public static int System_is_under_maintenance = 10200;
            public static int Invalid_request_parameter = 40000;
            public static int Server_error = 50000;

        }

        public static class CasinoPathPost
        {
            public static string CheckOrCreate = "/CheckOrCreate";
            public static string Login = "/Login";
            public static string LoginTrial = "/LoginTrial";
            public static string Logout = "/Logout";
            public static string GetPlayerSetting = "/GetPlayerSetting";
            public static string ModifyPlayerSetting = "/ModifyPlayerSetting";
            public static string GetAgentHandicaps = "/GetAgentHandicaps";
            public static string SetDefaultHandicapsForCreatingPlayer = "/SetDefaultHandicapsForCreatingPlayer";
            public static string GetGameTables = "/GetGameTables";
            public static string GetMaintenanceState = "/GetMaintenanceState";
            public static string SetMaintenanceState = "/SetMaintenanceState";
            public static string QueryWinningBetType = "/QueryWinningBetType";

            public static string QueryBetRecordByBetNum = "/QueryBetRecordByBetNum";
            public static string QuickQueryBetRecords = "/QuickQueryBetRecords";
            public static string PagingQueryBetRecords = "/PagingQueryBetRecords";
            public static string QueryModifiedBetRecords = "/QueryModifiedBetRecords";
            public static string QuerySumHistories = "/QuerySumHistories";
            public static string QueryOneDaySumHistories = "/QueryOneDaySumHistories";
            public static string PagingQueryEventRecords = "/PagingQueryEventRecords";
            public static string PagingQueryBetRecordsByPlayer = "/PagingQueryBetRecordsByPlayer";

        }

        public static class CasinoPartnerPath
        {
            public static string GetBalance = "getbalance";
            public static string Transfer = "transfer";
            public static string CancelTransfer = "canceltransfer";

        }

        public static class CasinoBetStatus
        {
            public static int Betting = 100;
            public static int BetFailed = 101;
            public static int NotSettled = 110;
            public static int Settled = 111;
            public static int Refund = 120;

            public static int[] BetRunning = new int[] { Betting, NotSettled };
            public static int[] BetCompleted = new int[] { BetFailed, Settled, Refund };
        }

        public static class CasinoBetType
        {
            public static int Bet = 10;
            public static int Settle = 20;
            public static int ManualSettle = 21;
            public static int TransferIn = 30;
            public static int TransferOut = 31;
            public static int EventSettle = 40;
        }
    }
}

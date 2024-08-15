namespace Lottery.Core.Helpers
{
    public static class PartnerHelper
    {
        public static class CAPartnerKey
        {
            public static string CABookieSettingKey = "CABSKey";
        }

        public static class CAReponseCode
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
    }
}

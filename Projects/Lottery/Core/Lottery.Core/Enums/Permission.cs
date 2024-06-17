namespace Lottery.Core.Enums
{
    public static class Permission
    {
        public static class MemberInformation
        {
            public const string View = "[AV]";
            public const string Update = "[AU]";
            public const string FullControl = "[AFC]";
        }

        public static class BetAndForecast
        {
            public const string BetForecast = "[BF]";
        }

        public static class Report
        {
            public const string Reports = "[R]";
        }

        public static class BetList
        {
            public const string BetLists = "[BL]";
        }

        public static class ViewLog
        {
            public const string ViewLogs = "[VL]";
        }

        public static class Management
        {
            public const string BetKinds = "[BK]";
            public const string Channels = "[CH]";
            public const string Prizes = "[PR]";
            public const string Matches = "[MA]";
            public const string MatchResults = "[MR]";
            public const string DefaultBetSetting = "[DO]";
            public const string DefaultPt = "[DPT]";

            public static class OddsTable
            {
                public const string MienBac_De = "[OTND]";
                public const string MienBac_Lo = "[OTNL]";
                public const string MienBac_Others = "[OTNO]";

                public const string MienTrung_De = "[OTCD]";
                public const string MienTrung_Lo = "[OTCL]";
                public const string MienTrung_Others = "[OTCO]";

                public const string MienNam_De = "[OTSD]";
                public const string MienNam_Lo = "[OTSL]";
                public const string MienNam_Others = "[OTSO]";
            }

            public const string Sessions = "[SE]";
            public const string Audits = "[AD]";
            public const string AdvancedTickets = "[AT]";
        }
    }
}

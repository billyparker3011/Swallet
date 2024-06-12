namespace Lottery.Core.Configs
{
    public static class CachingConfigs
    {
        public const string RedisConnectionForApp = "RedisForApp";
        public const int HashStructureMaxLength = 1000;
        public const int ExpiredTimeKeyInHours = 48;
        public const string RedisFormatDateTime = "MM/dd/yyyy HH:mm:ss";
        public const string RunningMatchKey = "running.match";
        public const string SessionKeyByRole = "session.{0}.{1}";   //  {0} = RoleId, {1} = AgentId/PlayerId

        //  Begin: Player Points By Match & Number
        public const string PlayerPointsByMatchAndNumberKey = "points.player.{0}.{1}.{2}"; //  <PlayerId / 1000>, <MatchId>, <Number>
        public const string PlayerPointsByMatchAndNumberValueOfKey = "points.{0}"; //  <PlayerId % 1000>
        //  End: Player Points By Match & Number

        //  Begin: Player Outs
        public const string PlayerOutsByMatchKey = "outs.player.{0}.{1}"; //  <PlayerId / 1000>, <MatchId>
        public const string PlayerOutsByMatchValueOfKey = "outs.{0}"; //  <PlayerId % 1000>
        //  End: Player Outs

        //  Begin: Player Given Credit
        public const string PlayerGivenCreditKey = "given-credit.player.{0}"; //  <PlayerId / 1000>
        public const string PlayerGivenCreditValueOfKey = "credit.{0}"; //  <PlayerId % 1000>
        //  End: Player Given Credit

        //  Begin: Min Bet
        public const string MinBetPlayerKey = "min-bet.player.{0}.{1}"; //  <PlayerId / 1000>.<BetKindId>
        public const string MinBetPlayerValueOfKey = "min-bet.{0}"; //  <PlayerId % 1000>
        //  End: Min Bet

        //  Begin: Max Bet
        public const string MaxBetPlayerKey = "max-bet.player.{0}.{1}"; //  <PlayerId / 1000>.<BetKindId>
        public const string MaxBetPlayerValueOfKey = "max-bet.{0}"; //  <PlayerId % 1000>
        //  End: Max Bet

        //  Begin: Max Per Number
        public const string MaxPerNumberPlayerKey = "max-per-number.player.{0}.{1}"; //  <PlayerId / 1000>.<BetKindId>
        public const string MaxPerNumberPlayerValueOfKey = "max-per-number.{0}"; //  <PlayerId % 1000>
        //  End: Max Per Number

        //  Begin: Player Odds By BetKind
        public const string PlayerOddsByBetKindMainKey = "odds.player.{0}.{1}"; //  <PlayerId / 1000>.<BetKindId>
        public const string PlayerOddsByBetKindSubKey = "odds.{0}"; //  <PlayerId % 1000>
        //  End: Player Odds By BetKind

        //  Begin: Default Player Odds By Match
        public const string PlayerOddsByMatchMainKey = "odds.player.{0}.{1}.{2}.{3}"; //  <PlayerId / 1000>.<MatchId>.<BetKindId>.<Number>
        public const string PlayerOddsByMatchSubKey = "odds.{0}"; //  <PlayerId % 1000>
        //  End: Default Player Odds By Match

        //  Begin: Default Mixed Player Odds By Match
        public const string MixedPlayerOddsByMatchMainKey = "odds.player.{0}.{1}.{2}.00-99.{3}"; //  <PlayerId / 1000>.<MatchId>.<OriginBetKindId>.00-99.<BetKindId>
        public const string MixedPlayerOddsByMatchSubKey = "odds.{0}"; //  <PlayerId % 1000>
        //  End: Default Player Odds By Match

        //  Begin: Stats by Match
        public const string PointStatsKeyByMatchBetKindNumberMainKey = "table-point.{0}.{1}.{2}"; //  <MatchId / 1000>.<BetKindId>.<Number>
        public const string PointStatsKeyByMatchBetKindNumberSubKey = "point.{0}"; //  <MatchId % 1000>

        public const string PayoutStatsKeyByMatchBetKindNumberMainKey = "table-payout.{0}.{1}.{2}"; //  <MatchId / 1000>.<BetKindId>.<Number>
        public const string PayoutStatsKeyByMatchBetKindNumberSubKey = "payout.{0}"; //  <MatchId % 1000>

        public const string RateStatsKeyByMatchBetKindNumberMainKey = "table-rate.{0}.{1}.{2}"; //  <MatchId / 1000>.<BetKindId>.<Number>
        public const string RateStatsKeyByMatchBetKindNumberSubKey = "rate.{0}"; //  <MatchId % 1000>
        //  End: Stats by Match
    }
}

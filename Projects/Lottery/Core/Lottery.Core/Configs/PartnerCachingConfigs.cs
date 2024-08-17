namespace Lottery.Core.Configs
{
    public static class PartnerCachingConfigs
    {
        // Begin: Client URL
        public const string Ga28ClientUrlByPlayerIdMainKey = "ga28-client-url.{0}"; // <PlayerId / 1000>
        public const string Ga28ClientUrlByPlayerIdSubKey = "url.{0}"; // <PlayerId % 1000>
        // End: Client URL

        // Begin: Partner Token
        public const string Ga28TokenByPlayerIdMainKey = "ga28-token.{0}"; // <PlayerId / 1000>
        public const string Ga28TokenByPlayerIdSubKey = "token.{0}"; // <PlayerId % 1000>
        // End: Partner Token

        // Begin: Client URL
        public const string CasinoClientUrlByPlayerIdMainKey = "ca-client-url.{0}"; // <PlayerId / 1000>
        public const string CasinoClientUrlByPlayerIdSubKey = "url.{0}"; // <PlayerId % 1000>
        // End: Client URL

        // Begin: Partner Token
        public const string CasinoTokenByPlayerIdMainKey = "ca-token.{0}"; // <PlayerId / 1000>
        public const string CasinoTokenByPlayerIdSubKey = "token.{0}"; // <PlayerId % 1000>
        // End: Partner Token
    }
}

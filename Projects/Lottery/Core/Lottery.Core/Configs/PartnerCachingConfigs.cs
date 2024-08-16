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
        public const string CAClientUrlByPlayerIdMainKey = "ca-client-url.{0}"; // <PlayerId / 1000>
        public const string CAClientUrlByPlayerIdSubKey = "url.{0}"; // <PlayerId % 1000>
        // End: Client URL

        // Begin: Partner Token
        public const string CATokenByPlayerIdMainKey = "ca-token.{0}"; // <PlayerId / 1000>
        public const string CATokenByPlayerIdSubKey = "token.{0}"; // <PlayerId % 1000>
        // End: Partner Token
    }
}

namespace Lottery.Core.Configs
{
    public static class WinloseCachingConfigs
    {
        //  Begin: Winlose by sport type
        public const string WinloseBySportKindMainKey = "sport-{0}-{1}.{2}"; // <SportKindId>-<KO(MM)-KO(DD)-KO(YYYY)>.<PlayerId / 1000>
        public const string WinloseBySportKindSubKey = "winlose.{0}"; // <PlayerId % 1000>
        //  End
    }
}

using UAParser;

namespace HnMicro.Framework.Helpers
{
    public static class BrowserHelper
    {
        public static string GetBrowser(this string userAgent)
        {
            userAgent = string.IsNullOrEmpty(userAgent) ? string.Empty : userAgent.Trim();
            if (string.IsNullOrEmpty(userAgent)) return string.Empty;
            var uaParser = Parser.GetDefault();
            var reader = uaParser.Parse(userAgent);
            return reader != null ? reader.UA.ToString() : string.Empty;
        }

        public static string GetPlatform(this string userAgent)
        {
            userAgent = string.IsNullOrEmpty(userAgent) ? string.Empty : userAgent.Trim();
            if (string.IsNullOrEmpty(userAgent)) return string.Empty;
            var uaParser = Parser.GetDefault();
            var reader = uaParser.Parse(userAgent);
            return reader != null ? reader.OS.ToString() : string.Empty;
        }
    }
}

using HnMicro.Framework.Logger.Options;

namespace HnMicro.Framework.Logger.Helpers
{
    public static class LoggerHelper
    {
        public static string GetDefaultLevelLogger(string name = "Default")
        {
            return $"{LoggingOption.AppSettingName}:{LoggingOption.LogLevel}:{name}";
        }
    }
}

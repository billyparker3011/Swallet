namespace HnMicro.Framework.Logger.Configs
{
    public static class LoggerConfig
    {
        public const string LoggerServiceConfigChannel = "logger-service-channel";
        public const string LoggerServiceConfigServerName = "RedisForLoggingSubscription";  //  It's the same ServerName in CachingConnections:RedisConnections configuration.
    }
}

namespace HnMicro.Module.Caching.ByRedis.Options
{
    public class RedisConfigurationOption
    {
        public const string AppSettingName = "CachingConnections";

        public List<RedisConfigurationItemOption> RedisConnections { get; set; }
    }
}

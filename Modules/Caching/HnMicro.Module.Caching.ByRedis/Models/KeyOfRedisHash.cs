namespace HnMicro.Module.Caching.ByRedis.Models
{
    public class KeyOfRedisHash
    {
        public string MainKey { get; set; }
        public string SubKey { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}

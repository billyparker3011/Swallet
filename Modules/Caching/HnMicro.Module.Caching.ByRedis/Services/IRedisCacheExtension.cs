using StackExchange.Redis;

namespace HnMicro.Module.Caching.ByRedis.Services
{
    public interface IRedisCacheExtension
    {
        #region List
        void Enqueue<T>(string key, T value, string to = "", int database = -1);
        Task EnqueueAsync<T>(string key, T value, string to = "", int database = -1);

        T Dequeue<T>(string key, string to = "", int database = -1);
        Task<T> DequeueAsync<T>(string key, string to = "", int database = -1);

        List<T> PeekRange<T>(string key, long start = 0, long stop = -1, string to = "", int database = -1) where T : class;
        Task<List<T>> PeekRangeAsync<T>(string key, long start = 0, long stop = -1, string to = "", int database = -1) where T : class;
        #endregion

        #region Set
        bool SetAdd<T>(string key, T value, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1);
        Task<bool> SetAddAsync<T>(string key, T value, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1);

        long SetRemove<T>(string key, List<T> values, string to = "", int database = -1);
        Task<long> SetRemoveAsync<T>(string key, List<T> values, string to = "", int database = -1);

        List<T> SetMembers<T>(string key, string to = "", int database = -1) where T : class;
        Task<List<T>> SetMembersAsync<T>(string key, string to = "", int database = -1) where T : class;

        bool SetContains<T>(string key, T value, string to = "", int database = -1);
        Task<bool> SetContainsAsync<T>(string key, T value, string to = "", int database = -1);
        #endregion

        #region Sortedset
        bool SortedSetAdd(string key, string member, double score, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1);
        Task<bool> SortedSetAddAsync(string key, string member, double score, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1);

        long SortedSetRemove(string key, List<string> members, string to = "", int database = -1);
        Task<long> SortedSetRemoveAsync(string key, List<string> members, string to = "", int database = -1);

        double SortedSetIncrement(string key, string member, double value, string to = "", int database = -1);
        Task<double> SortedSetIncrementAsync(string key, string member, double value, string to = "", int database = -1);

        double SortedSetDecrement(string key, string member, double value, string to = "", int database = -1);
        Task<double> SortedSetDecrementAsync(string key, string member, double value, string to = "", int database = -1);

        Dictionary<string, double> SortedSetRangeByRankWithScores(string key, long start = 0, long stop = -1, Order order = Order.Ascending, string to = "", int database = -1);
        Task<Dictionary<string, double>> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, Order order = Order.Ascending, string to = "", int database = -1);

        Dictionary<string, double> SortedSetRangeByScoreWithScores(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, string to = "", int database = -1);
        Task<Dictionary<string, double>> SortedSetRangeByScoreWithScoresAsync(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, string to = "", int database = -1);
        #endregion

        #region Hash
        Dictionary<string, string> HashGet(string key, string to = "", int database = -1);
        Task<Dictionary<string, string>> HashGetAsync(string key, string to = "", int database = -1);

        Dictionary<string, string> HashGetFields(string key, List<string> fields, string to = "", int database = -1);
        Task<Dictionary<string, string>> HashGetFieldsAsync(string key, List<string> fields, string to = "", int database = -1);

        void HashSet(string key, Dictionary<string, string> entries, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1);
        Task HashSetAsync(string key, Dictionary<string, string> entries, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1);

        void HashSetFields(string key, Dictionary<string, string> fields, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1);
        Task HashSetFieldsAsync(string key, Dictionary<string, string> fields, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1);

        bool HashDeleteFields(string key, List<string> fields, string to = "", int database = -1);
        Task<bool> HashDeleteFieldsAsync(string key, List<string> fields, string to = "", int database = -1);
        #endregion

        #region Advanced (Pub/Sub)
        long Publish(RedisChannel channel, string msg, string to = "");
        Task<long> PublishAsync(RedisChannel channel, string msg, string to = "");

        void Subscribe(RedisChannel channel, Action<RedisChannel, string> handler, string to = "");
        Task SubscribeAsync(RedisChannel channel, Action<RedisChannel, string> handler, string to = "");

        void ExecuteBatch(List<Action> operations, string to = "", int database = -1);
        Task ExecuteBatchAsync(List<Action> operations, string to = "", int database = -1);
        #endregion

        #region Key
        bool Exists(string key, string to = "", int database = -1);
        Task<bool> ExistsAsync(string key, string to = "", int database = -1);
        List<string> GetAllKeys(string to = "");
        bool Expire(string key, TimeSpan expiredTimeInSeconds, string to = "", int database = -1);
        Task<bool> ExpireAsync(string key, TimeSpan expiredTimeInSeconds, string to = "", int database = -1);
        #endregion
    }
}

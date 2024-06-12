using HnMicro.Framework.Configs;
using HnMicro.Framework.Exceptions;
using HnMicro.Module.Caching.ByRedis.Helpers;
using HnMicro.Module.Caching.ByRedis.Options;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace HnMicro.Module.Caching.ByRedis.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly ILogger<RedisCacheService> _logger;
        private readonly RedisConfigurationOption _configurationOption;
        private readonly Dictionary<string, ConnectionMultiplexer> _connections = new();
        private Timer _timer;

        public RedisCacheService(ILogger<RedisCacheService> logger, RedisConfigurationOption configurationOption)
        {
            _logger = logger;
            _configurationOption = configurationOption;

            InitConnections();
            InitTimer();
        }

        private void InitTimer()
        {
            _timer = new Timer(CallBack, null, SystemConfigs.IntervalTimerInMilliseconds, Timeout.Infinite);
        }

        private void CallBack(object state)
        {
            //  Stop Timer
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            foreach (var item in _connections)
            {
                try
                {
                    item.Value.GetDatabase().Ping();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Cannot ping {0}.", item.Key);
                }
            }
            //  Start Timer again
            _timer.Change(SystemConfigs.IntervalTimerInMilliseconds, Timeout.Infinite);
        }

        private void InitConnections()
        {
            if (_configurationOption.RedisConnections == null) return;
            foreach (var item in _configurationOption.RedisConnections)
            {
                _connections[item.ServerName] = ConnectionMultiplexer.Connect(item.ConnectionStrings);
            }
        }

        private ConnectionMultiplexer GetConnectionFrom(string serverName)
        {
            if (string.IsNullOrEmpty(serverName))
            {
                if (_connections.Count == 0)
                    throw new HnMicroException("Cannot find any Redis connection.");
                return _connections.Values.FirstOrDefault();
            }
            if (!_connections.ContainsKey(serverName))
                throw new HnMicroException("Cannot find Redis connection via ServerName.");
            return _connections[serverName];
        }

        private IDatabase GetDatabase(string to, int database = -1)
        {
            var connection = GetConnectionFrom(to);
            return connection.GetDatabase(database);
        }

        #region String
        public bool Exists(string key, string to = "")
        {
            return GetDatabase(to).KeyExists(key);
        }

        public async Task<bool> ExistsAsync(string key, string to = "")
        {
            return await GetDatabase(to).KeyExistsAsync(key);
        }

        public T GetData<T>(string key, string to = "")
        {
            return GetDatabase(to).StringGet(key).ToObject<T>();
        }

        public async Task<T> GetDataAsync<T>(string key, string to = "")
        {
            return (await GetDatabase(to).StringGetAsync(key)).ToObject<T>();
        }

        public bool Remove(string key, string to = "")
        {
            return GetDatabase(to).KeyDelete(key);
        }

        public async Task<bool> RemoveAsync(string key, string to = "")
        {
            return await GetDatabase(to).KeyDeleteAsync(key);
        }

        public bool SetData<T>(string key, T value, string to = "")
        {
            return GetDatabase(to).StringSet(key, value.ToRedisValue());
        }

        public async Task<bool> SetDataAsync<T>(string key, T value, string to = "")
        {
            return await GetDatabase(to).StringSetAsync(key, value.ToRedisValue());
        }

        public bool SetData<T>(string key, T value, TimeSpan expiredTimeInSeconds, string to = "")
        {
            return GetDatabase(to).StringSet(key, value.ToRedisValue(), expiredTimeInSeconds);
        }

        public async Task<bool> SetDataAsync<T>(string key, T value, TimeSpan expiredTimeInSeconds, string to = "")
        {
            return await GetDatabase(to).StringSetAsync(key, value.ToRedisValue(), expiredTimeInSeconds);
        }
        #endregion

        #region Hash
        public Dictionary<string, string> HashGet(string key, string to = "", int database = -1)
        {
            return GetDatabase(to, database).HashGetAll(key).ToDictionaryValue();
        }

        public async Task<Dictionary<string, string>> HashGetAsync(string key, string to = "", int database = -1)
        {
            return (await GetDatabase(to, database).HashGetAllAsync(key)).ToDictionaryValue();
        }

        public Dictionary<string, string> HashGetFields(string key, List<string> fields, string to = "", int database = -1)
        {
            return GetDatabase(to, database).HashGet(key, fields.ToRedisValues()).ToDictionaryValue(fields);
        }

        public async Task<Dictionary<string, string>> HashGetFieldsAsync(string key, List<string> fields, string to = "", int database = -1)
        {
            return (await GetDatabase(to, database).HashGetAsync(key, fields.ToRedisValues())).ToDictionaryValue(fields);
        }

        public void HashSet(string key, Dictionary<string, string> entries, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1)
        {
            var vals = entries.ToHashEntries();
            if (vals == null) return;

            var participantDatabase = GetDatabase(to, database);
            participantDatabase.HashSet(key, vals);

            if (expiredTimeInSeconds == null) return;
            participantDatabase.KeyExpire(key, expiredTimeInSeconds);
        }

        public async Task HashSetAsync(string key, Dictionary<string, string> entries, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1)
        {
            var vals = entries.ToHashEntries();
            if (vals == null) return;

            var participantDatabase = GetDatabase(to, database);
            await participantDatabase.HashSetAsync(key, vals);

            if (expiredTimeInSeconds == null) return;
            await participantDatabase.KeyExpireAsync(key, expiredTimeInSeconds);
        }

        public void HashSetFields(string key, Dictionary<string, string> fields, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1)
        {
            if (fields.Count == 0) return;
            var entries = HashGet(key, to, database);
            if (entries == null)
            {
                entries = new Dictionary<string, string>();
                foreach (var item in fields) entries[item.Key] = item.Value;
            }
            else
            {
                foreach (var field in fields)
                {
                    if (!entries.ContainsKey(field.Key)) continue;
                    entries[field.Key] = field.Value;
                }
            }
            HashSet(key, entries, expiredTimeInSeconds, to, database);
        }

        public async Task HashSetFieldsAsync(string key, Dictionary<string, string> fields, TimeSpan? expiredTimeInSeconds = null, string to = "", int database = -1)
        {
            if (fields.Count == 0) return;
            var entries = await HashGetAsync(key, to, database);
            if (entries == null)
            {
                entries = new Dictionary<string, string>();
                foreach (var item in fields) entries[item.Key] = item.Value;
            }
            else
            {
                foreach (var field in fields)
                {
                    entries[field.Key] = field.Value;
                }
            }
            await HashSetAsync(key, entries, expiredTimeInSeconds, to, database);
        }

        public bool HashDeleteFields(string key, List<string> fields, string to = "", int database = -1)
        {
            if (fields.Count == 0) return true;
            var participantDatabase = GetDatabase(to, database);
            var success = true;
            foreach (var field in fields)
            {
                if (!participantDatabase.HashDelete(key, field)) success = false;
            }
            return success;
        }

        public async Task<bool> HashDeleteFieldsAsync(string key, List<string> fields, string to = "", int database = -1)
        {
            if (fields.Count == 0) return true;
            var participantDatabase = GetDatabase(to, database);
            var success = true;
            foreach (var field in fields)
            {
                if (!await participantDatabase.HashDeleteAsync(key, field)) success = false;
            }
            return success;
        }
        #endregion

        #region List
        public void Enqueue<T>(string key, T value, string to = "", int database = -1)
        {
            GetDatabase(to, database).ListRightPush(key, value.ToRedisValue());
        }

        public async Task EnqueueAsync<T>(string key, T value, string to = "", int database = -1)
        {
            await GetDatabase(to, database).ListRightPushAsync(key, value.ToRedisValue());
        }

        public T Dequeue<T>(string key, string to = "", int database = -1)
        {
            return GetDatabase(to, database).ListLeftPop(key).ToObject<T>();
        }

        public async Task<T> DequeueAsync<T>(string key, string to = "", int database = -1)
        {
            return (await GetDatabase(to, database).ListLeftPopAsync(key)).ToObject<T>();
        }

        public List<T> PeekRange<T>(string key, long start = 0, long stop = -1, string to = "", int database = -1) where T : class
        {
            return GetDatabase(to, database).ListRange(key, start, stop).ToListObject<T>().ToList();
        }

        public async Task<List<T>> PeekRangeAsync<T>(string key, long start = 0, long stop = -1, string to = "", int database = -1) where T : class
        {
            return (await GetDatabase(to, database).ListRangeAsync(key, start, stop)).ToListObject<T>().ToList();
        }
        #endregion

        #region Set
        public bool SetAdd<T>(string key, T value, string to = "", int database = -1)
        {
            return GetDatabase(to, database).SetAdd(key, value.ToRedisValue());
        }

        public async Task<bool> SetAddAsync<T>(string key, T value, string to = "", int database = -1)
        {
            return await GetDatabase(to, database).SetAddAsync(key, value.ToRedisValue());
        }

        public long SetRemove<T>(string key, List<T> values, string to = "", int database = -1)
        {
            return GetDatabase(to, database).SetRemove(key, values.ToRedisValues());
        }

        public async Task<long> SetRemoveAsync<T>(string key, List<T> values, string to = "", int database = -1)
        {
            return await GetDatabase(to, database).SetRemoveAsync(key, values.ToRedisValues());
        }

        public List<T> SetMembers<T>(string key, string to = "", int database = -1) where T : class
        {
            return GetDatabase(to, database).SetMembers(key).ToListObject<T>().ToList();
        }

        public async Task<List<T>> SetMembersAsync<T>(string key, string to = "", int database = -1) where T : class
        {
            return (await GetDatabase(to, database).SetMembersAsync(key)).ToListObject<T>().ToList();
        }

        public bool SetContains<T>(string key, T value, string to = "", int database = -1)
        {
            return GetDatabase(to, database).SetContains(key, value.ToRedisValue());
        }

        public async Task<bool> SetContainsAsync<T>(string key, T value, string to = "", int database = -1)
        {
            return await GetDatabase(to, database).SetContainsAsync(key, value.ToRedisValue());
        }
        #endregion

        #region SortedSet
        public bool SortedSetAdd(string key, string member, double score, string to = "", int database = -1)
        {
            return GetDatabase(to, database).SortedSetAdd(key, member, score);
        }

        public async Task<bool> SortedSetAddAsync(string key, string member, double score, string to = "", int database = -1)
        {
            return await GetDatabase(to, database).SortedSetAddAsync(key, member, score);
        }

        public long SortedSetRemove(string key, List<string> members, string to = "", int database = -1)
        {
            return GetDatabase(to, database).SortedSetRemove(key, members.ToRedisValues());
        }

        public async Task<long> SortedSetRemoveAsync(string key, List<string> members, string to = "", int database = -1)
        {
            return await GetDatabase(to, database).SortedSetRemoveAsync(key, members.ToRedisValues());
        }

        public double SortedSetIncrement(string key, string member, double value, string to = "", int database = -1)
        {
            return GetDatabase(to, database).SortedSetIncrement(key, member, value);
        }

        public async Task<double> SortedSetIncrementAsync(string key, string member, double value, string to = "", int database = -1)
        {
            return await GetDatabase(to, database).SortedSetIncrementAsync(key, member, value);
        }

        public double SortedSetDecrement(string key, string member, double value, string to = "", int database = -1)
        {
            return GetDatabase(to, database).SortedSetDecrement(key, member, value);
        }

        public async Task<double> SortedSetDecrementAsync(string key, string member, double value, string to = "", int database = -1)
        {
            return await GetDatabase(to, database).SortedSetDecrementAsync(key, member, value);
        }

        public Dictionary<string, double> SortedSetRangeByRankWithScores(string key, long start = 0, long stop = -1, Order order = Order.Ascending, string to = "", int database = -1)
        {
            return GetDatabase(to, database).SortedSetRangeByRankWithScores(key, start, stop, order).ToDictionaryValue();
        }

        public async Task<Dictionary<string, double>> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, Order order = Order.Ascending, string to = "", int database = -1)
        {
            return (await GetDatabase(to, database).SortedSetRangeByRankWithScoresAsync(key, start, stop, order)).ToDictionaryValue();
        }

        public Dictionary<string, double> SortedSetRangeByScoreWithScores(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, string to = "", int database = -1)
        {
            return GetDatabase(to, database).SortedSetRangeByScoreWithScores(key, start, stop, exclude, order, skip, take).ToDictionaryValue();
        }

        public async Task<Dictionary<string, double>> SortedSetRangeByScoreWithScoresAsync(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, string to = "", int database = -1)
        {
            return (await GetDatabase(to, database).SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take)).ToDictionaryValue();
        }
        #endregion

        #region Key
        public bool Exists(string key, string to = "", int database = -1)
        {
            return GetDatabase(to, database).KeyExists(key);
        }

        public async Task<bool> ExistsAsync(string key, string to = "", int database = -1)
        {
            return await GetDatabase(to, database).KeyExistsAsync(key);
        }

        public List<string> GetAllKeys(string to = "")
        {
            var connection = GetConnectionFrom(to);
            return connection.GetEndPoints().Select(f => connection.GetServer(f)).SelectMany(f => f.Keys().ToStrings()).ToList();
        }

        public bool Expire(string key, TimeSpan expiredTimeInSeconds, string to = "", int database = -1)
        {
            return GetDatabase(to, database).KeyExpire(key, expiredTimeInSeconds);
        }

        public async Task<bool> ExpireAsync(string key, TimeSpan expiredTimeInSeconds, string to = "", int database = -1)
        {
            return await GetDatabase(to, database).KeyExpireAsync(key, expiredTimeInSeconds);
        }
        #endregion

        #region Advanced (Pub/Sub)
        public long Publish(RedisChannel channel, string msg, string to = "")
        {
            var connection = GetConnectionFrom(to);
            return connection.GetSubscriber().Publish(channel, msg);
        }

        public async Task<long> PublishAsync(RedisChannel channel, string msg, string to = "")
        {
            var connection = GetConnectionFrom(to);
            return await connection.GetSubscriber().PublishAsync(channel, msg);
        }

        public void Subscribe(RedisChannel channel, Action<RedisChannel, string> handler, string to = "")
        {
            var connection = GetConnectionFrom(to);
            connection.GetSubscriber().Subscribe(channel, (hChannel, hMsg) => handler(hChannel, hMsg));
        }

        public async Task SubscribeAsync(RedisChannel channel, Action<RedisChannel, string> handler, string to = "")
        {
            var connection = GetConnectionFrom(to);
            await connection.GetSubscriber().SubscribeAsync(channel, (hChannel, hMsg) => handler(hChannel, hMsg));
        }

        public void ExecuteBatch(List<Action> operations, string to = "", int database = -1)
        {
            var batch = GetDatabase(to, database).CreateBatch();
            foreach (var operation in operations) operation();
            batch.Execute();
        }

        public Task ExecuteBatchAsync(List<Action> operations, string to = "", int database = -1)
        {
            return Task.Run(() =>
            {
                var batch = GetDatabase(to, database).CreateBatch();
                foreach (var operation in operations) operation();
                batch.Execute();
            });
        }
        #endregion
    }
}

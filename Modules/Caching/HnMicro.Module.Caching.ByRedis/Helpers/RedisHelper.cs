using Newtonsoft.Json;
using StackExchange.Redis;

namespace HnMicro.Module.Caching.ByRedis.Helpers
{
    public static class RedisHelper
    {
        public static IEnumerable<string> ToStrings(this IEnumerable<RedisKey> keys)
        {
            var redisKeys = keys as RedisKey[] ?? keys.ToArray();
            return !redisKeys.Any() ? null : redisKeys.Select(k => (string)k);
        }

        public static RedisValue ToRedisValue<T>(this T value)
        {
            if (value == null) return RedisValue.Null;

            return value switch
            {
                ValueType => value.ToString(),
                string s => s,
                _ => JsonConvert.SerializeObject(value)
            };
        }

        public static RedisValue[] ToRedisValues<T>(this IEnumerable<T> values)
        {
            var enumerable = values as T[] ?? values.ToArray();
            return !enumerable.Any() ? null : enumerable.Select(v => v.ToRedisValue()).ToArray();
        }

        public static T ToObject<T>(this RedisValue value)
        {
            if (!value.HasValue) return default;
            if (typeof(T).IsSubclassOf(typeof(ValueType)) || typeof(T) == typeof(string)) return (T)Convert.ChangeType(value.ToString(), typeof(T));
            return JsonConvert.DeserializeObject<T>(value.ToString());
        }

        public static IEnumerable<T> ToListObject<T>(this IEnumerable<RedisValue> values)
        {
            var redisValues = values as RedisValue[] ?? values.ToArray();
            return !redisValues.Any() ? new List<T>() : redisValues.Select(v => v.ToObject<T>());
        }

        public static HashEntry[] ToHashEntries(this Dictionary<string, string> entries)
        {
            if (entries == null || !entries.Any()) return null;

            var es = new HashEntry[entries.Count];
            for (var i = 0; i < entries.Count; i++)
            {
                var name = entries.Keys.ElementAt(i);
                var value = entries[name];
                es[i] = new HashEntry(name, value);
            }
            return es;
        }

        public static Dictionary<string, string> ToDictionaryValue(this IEnumerable<HashEntry> entries)
        {
            var hashEntries = entries as HashEntry[] ?? entries.ToArray();
            if (!hashEntries.Any()) return null;

            var dict = new Dictionary<string, string>();
            foreach (var entry in hashEntries) dict[entry.Name] = entry.Value;

            return dict;
        }

        public static Dictionary<string, string> ToDictionaryValue(this RedisValue[] hashValues, IEnumerable<string> fields)
        {
            var enumerable = fields as string[] ?? fields.ToArray();
            if (hashValues == null || !hashValues.Any() || !enumerable.Any()) return null;

            var dict = new Dictionary<string, string>();
            for (var i = 0; i < enumerable.Count(); i++)
                dict[enumerable.ElementAt(i)] = hashValues[i];
            return dict;
        }

        public static Dictionary<string, double> ToDictionaryValue(this IEnumerable<SortedSetEntry> entries)
        {
            var sortedSetEntries = entries as SortedSetEntry[] ?? entries.ToArray();
            if (!sortedSetEntries.Any()) return null;

            var dict = new Dictionary<string, double>();
            foreach (var entry in sortedSetEntries) dict[entry.Element] = entry.Score;
            return dict;
        }
    }
}

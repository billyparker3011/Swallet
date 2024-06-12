namespace HnMicro.Framework.Caching.Services
{
    public interface ICacheService
    {
        T GetData<T>(string key, string to = "");
        Task<T> GetDataAsync<T>(string key, string to = "");

        bool SetData<T>(string key, T value, string to = "");
        Task<bool> SetDataAsync<T>(string key, T value, string to = "");

        bool SetData<T>(string key, T value, TimeSpan expiredTimeInSeconds, string to = "");
        Task<bool> SetDataAsync<T>(string key, T value, TimeSpan expiredTimeInSeconds, string to = "");

        bool Remove(string key, string to = "");
        Task<bool> RemoveAsync(string key, string to = "");

        bool Exists(string key, string to = "");
        Task<bool> ExistsAsync(string key, string to = "");
    }
}

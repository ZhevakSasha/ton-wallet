namespace TonWalletApi.Services.Caching
{
    public interface ICacheService
    {
        Task SetCacheAsync<T>(string key, T value, TimeSpan ttl);
        Task<T> GetCacheAsync<T>(string key);
        Task RemoveCacheAsync(string key);
    }
}

using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace TonWalletApi.Services.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetCacheAsync<T>(string key, T value, TimeSpan ttl)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            var serializedData = JsonConvert.SerializeObject(value);
            await _cache.SetStringAsync(key, serializedData, cacheOptions);
        }
        public async Task<T> GetCacheAsync<T>(string key)
        {
            var serializedData = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(serializedData))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(serializedData);
        }

        public async Task RemoveCacheAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}

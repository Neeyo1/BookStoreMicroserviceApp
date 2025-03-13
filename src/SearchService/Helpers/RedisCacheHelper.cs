using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace SearchService.Helpers;

public static class RedisCacheHelper
{
    public static async Task<T?> GetFromCacheAsync<T>(IDistributedCache cache, string cacheKey)
    {
        var cachedData = await cache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            Console.WriteLine("Cache HIT!");
            return JsonSerializer.Deserialize<T>(cachedData);
        }

        Console.WriteLine("Cache MISS!");
        return default;
    }

    public static async Task SetToCacheAsync<T>(IDistributedCache cache, string cacheKey, T data, int cacheDurationMinutes = 10)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheDurationMinutes)
        };

        var serializedData = JsonSerializer.Serialize(data);
        await cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
    }

    public static async Task RemoveKeysByPrefixAsync(IConnectionMultiplexer redis, string prefix)
    {
        var cache = redis.GetDatabase();
        var server = redis.GetServer(redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{prefix}*").ToArray();

        if (keys.Length != 0)
        {
            await cache.KeyDeleteAsync(keys);
        }
    }
}

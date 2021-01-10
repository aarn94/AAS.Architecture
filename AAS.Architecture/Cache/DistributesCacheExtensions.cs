using System;
using System.Threading.Tasks;
using AAS.Architecture.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AAS.Architecture.Cache
{
    public static class DistributesCacheExtensions
    {
        public static async Task<T> GetOrSetAsync<T>(this IDistributedCache cache, string key,
            Func<Task<T>> getValueFunc, DistributedCacheEntryOptions options = null)
        {
            var stringAsync = await cache.GetStringAsync(key).WithoutCapturingContext();
            if (stringAsync is {})
                return JsonConvert.DeserializeObject<T>(stringAsync);

            var sourceValue = await getValueFunc().WithoutCapturingContext();
            await cache.SetStringAsync(key, JsonConvert.SerializeObject(sourceValue), options).WithoutCapturingContext();

            return sourceValue;
        }
    }
}
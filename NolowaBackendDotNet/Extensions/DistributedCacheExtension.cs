using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace NolowaBackendDotNet.Extensions
{
    public static class DistributedCacheExtension
    {
        public static async Task SetRecoredAsync<T> (this IDistributedCache cache, 
                                                     string recoredId, T data, TimeSpan? absolutedExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = absolutedExpireTime ?? TimeSpan.FromMinutes(1.5),
                SlidingExpiration = unusedExpireTime,
            };

            var jsonData = JsonSerializer.Serialize(data);

            await cache.SetStringAsync(recoredId, jsonData, options);
        }

        public static async Task<T> GetRecoredAsync<T>(this IDistributedCache cache, string recoredId)
        {
            var jsonData = await cache.GetStringAsync(recoredId);

            if (jsonData.IsNull())
                return default(T);

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}

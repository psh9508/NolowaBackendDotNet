using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.Redis
{
    public interface ISearchRedis : IDistributedCache
    {
    }
    public class RedisCacheOptions3 : RedisCacheOptions
    {
    }

    public class SearchRedis : RedisCache, ISearchRedis
    {
        public SearchRedis(IOptions<RedisCacheOptions3> optionsAccessor) : base(optionsAccessor)
        {
        }
    }
}

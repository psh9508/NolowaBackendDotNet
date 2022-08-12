using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.Redis
{
    public interface IDirectMessageRedis : IDistributedCache
    {
    }
    public class RedisCacheOptions1 : RedisCacheOptions
    {
    }

    public class DirectMessageRedis : RedisCache, IDirectMessageRedis
    {
        public DirectMessageRedis(IOptions<RedisCacheOptions1> optionsAccessor) : base(optionsAccessor)
        {
        }
    }
}

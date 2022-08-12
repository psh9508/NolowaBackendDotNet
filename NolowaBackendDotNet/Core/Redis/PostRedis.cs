using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.Redis
{
    public interface IPostRedis : IDistributedCache
    {
    }
    public class RedisCacheOptions2 : RedisCacheOptions
    {
    }

    public class PostRedis : RedisCache, IPostRedis
    {
        public PostRedis(IOptions<RedisCacheOptions2> optionsAccessor) : base(optionsAccessor)
        {
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core.CacheMonitor;
using NolowaBackendDotNet.Core.Redis;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Models.IF;
using NolowaBackendDotNet.Services.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface IPostCacheService : ICacheService
    {
    }

    /// <summary>
    /// 데이터를 cache에 넣은 후 DB에 넣기위해 Channel에 넣는 역할을 한다.
    /// </summary>
    public class PostCacheService : IPostCacheService
    {
        //private readonly IDistributedCache _cache;
        private readonly IDirectMessageRedis _cacheDM;
        private readonly IPostRedis _cachePoset;
        private readonly IBackgroundCacheToDBTaskQueue _taskQueue;

        public PostCacheService(IDirectMessageRedis cacheDM, IPostRedis cachePost, IBackgroundCacheToDBTaskQueue taskQueue)
        {
            _cacheDM = cacheDM;
            _cachePoset = cachePost;
            _taskQueue = taskQueue;
        }

        public async Task SaveAndQueueToSaveDisk<T>(T data)
        {
            var randomId = Guid.NewGuid().ToString();

            // 캐시에 저장되면 바로 리턴
            await _cacheDM.SetRecoredAsync(randomId, data);

            _ = QueueToSaveDisk(new CacheQueueData()
            {
                Id = randomId,
                Data = data,
                InsertTryCount = 0
            });
        }

        public async Task RemoveItem(string key)
        {
            await _cacheDM.RemoveAsync(key);
        }

        private async Task QueueToSaveDisk(CacheQueueData data)
        {
            await _taskQueue.EnqueueAsync(data);
        }
    }
}

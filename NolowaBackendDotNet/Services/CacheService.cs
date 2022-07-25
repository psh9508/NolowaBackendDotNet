using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core.CacheMonitor;
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
    public class CacheQueueData
    {
        public string Id { get; set; }
        public dynamic Data { get; set; }
        public int InsertTryCount { get; set; }
    }

    public interface ICacheService
    {
        Task SaveAndQueueToSaveDisk<T>(T data);
        Task RemoveItem(string key);
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IBackgroundCacheToDBTaskQueue _taskQueue;

        public CacheService(IDistributedCache cache, IBackgroundCacheToDBTaskQueue taskQueue)
        {
            _cache = cache;
            _taskQueue = taskQueue;
        }

        public async Task SaveAndQueueToSaveDisk<T>(T data)
        {
            var randomId = Guid.NewGuid().ToString();

            // 캐시에 저장되면 바로 리턴
            await _cache.SetRecoredAsync(randomId, data);

            _ = QueueToSaveDisk(new CacheQueueData()
            {
                Id = randomId,
                Data = data,
                InsertTryCount = 0
            });
        }

        public async Task RemoveItem(string key)
        {
            await _cache.RemoveAsync(key);
        }

        private async Task QueueToSaveDisk(CacheQueueData data)
        {
            await _taskQueue.EnqueueBackgroundWorkItemAsync(data);
        }
    }
}

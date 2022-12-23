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
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface IPostCacheService : ICacheService
    {
        Task SaveAsync(string userId, string jsonData);
        Task SaveAsync<T>(string key, T value);
        Task<T> GetAsync<T>(string userId);
        Task RemoveAllAsync(string userId);
    }

    /// <summary>
    /// 데이터를 cache에 넣은 후 DB에 넣기위해 Channel에 넣는 역할을 한다.
    /// </summary>
    public class PostCacheService : IPostCacheService
    {
        private readonly IPostRedis _cache;
        private readonly IBackgroundCacheToDBTaskQueue _taskQueue;

        public PostCacheService(IPostRedis cache, IBackgroundCacheToDBTaskQueue taskQueue)
        {
            _cache = cache;
            _taskQueue = taskQueue;
        }

        public async Task SaveAsync(string userId, string jsonData)
        {
            try
            {
                await _cache.SetStringAsync(userId, jsonData);
            }
            catch (RedisConnectionException ex) 
            {
                throw;
            }
        }

        public async Task SaveAsync<T>(string key, T value)
        {
            try
            {
                byte[] StrByte = Encoding.UTF8.GetBytes(value.ToString());
                await _cache.SetAsync(key, StrByte);
            }
            catch (RedisConnectionException ex)
            {
                throw;
            }
        }

        public async Task<T> GetAsync<T>(string userId)
        {
            try
            {
                var redisJsonData = await _cache.GetStringAsync(userId);

                if (redisJsonData.IsNotVaild())
                    return default(T);

                await RemoveAllAsync(userId);

                return JsonSerializer.Deserialize<T>(redisJsonData, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });
            }
            catch (RedisConnectionException ex)
            {
                throw;
            }
        }

        public async Task SaveAndQueueToSaveDisk<T>(T data)
        {
            //var randomId = Guid.NewGuid().ToString();

            //// 캐시에 저장되면 바로 리턴
            //await _cacheDM.SetRecoredAsync(randomId, data);

            //_ = QueueToSaveDisk(new CacheQueueData()
            //{
            //    Id = randomId,
            //    Data = data,
            //    InsertTryCount = 0
            //});

            throw new NotImplementedException();
        }

        public async Task RemoveAllAsync(string userId)
        {
            try
            {
                await _cache.RemoveAsync(userId);
            }
            catch (RedisConnectionException ex)
            {
                throw;
            }
        }

        public async Task RemoveItem(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (RedisConnectionException ex)
            {
                throw;
            }
        }

        private async Task QueueToSaveDisk(CacheQueueData data)
        {
            try
            {
                await _taskQueue.EnqueueAsync(data);
            }
            catch (RedisConnectionException ex)
            {
                throw;
            }
        }
    }
}

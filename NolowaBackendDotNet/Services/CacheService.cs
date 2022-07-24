using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Models.IF;
using NolowaBackendDotNet.Services.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly NolowaContext _context;
        private static readonly ConcurrentQueue<CacheQueueData> _queue = new ConcurrentQueue<CacheQueueData>();
        private static bool _isQueueThreadStarted;
        private object _lock = new object();

        public CacheService(IDistributedCache cache, NolowaContext context)
        {
            _cache = cache;
            _context = context;

            lock(_lock)
            {
                if (_isQueueThreadStarted == false)
                    StartInsertThread();
            }
        }

        public async Task SaveAndQueueToSaveDisk<T>(T data)
        {
            var randomId = Guid.NewGuid().ToString();

            // 캐시에 저장되면 바로 리턴
            await _cache.SetRecoredAsync(randomId, data);

            QueueToSaveDisk(new CacheQueueData()
            {
                Id = randomId,
                Data = data,
                InsertTryCount = 0
            });
        }

        private void QueueToSaveDisk(CacheQueueData data)
        {
            _queue.Enqueue(data);
        }

        private void StartInsertThread()
        {
            // 혹시 Insert 작업이 재진입 시간보다 길어질 때 대비해서 재진입을 조절한다.
            bool isBeingOperated = false;
            var retryTime = TimeSpan.FromSeconds(10);

            Task.Run(async () =>    
            {
                _isQueueThreadStarted = true;

                while (true)
                {
                    CacheQueueData beInsertedData = null;

                    try
                    {
                        await Task.Delay(retryTime);

                        if (isBeingOperated == false)
                        {
                            isBeingOperated = true;

                            try
                            {
                                using (var context = new NolowaContext())
                                {
                                    for (int i = 0; i < _queue.Count; i++)
                                    {
                                        if (_queue.TryDequeue(out beInsertedData))
                                        {
                                            // 다형성으로 해결할 수 없을까?
                                            if (beInsertedData.Data is DirectMessage)
                                            {
                                                beInsertedData.InsertTryCount++;
                                                await context.DirectMessages.AddAsync(beInsertedData.Data);
                                            }

                                            if (context.ChangeTracker.HasChanges())
                                            {
                                                await context.SaveChangesAsync();
                                                await _cache.RemoveAsync(beInsertedData.Id); // DB에 저장되면 redis에서 지워준다.
                                            }
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                isBeingOperated = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if(beInsertedData.IsNotNull())
                        {
                            _queue.Enqueue(beInsertedData);
                        }
                        // 이 쓰레드는 절대 죽어선 안된다.
                        // beInsertedData 데이터 기록을 남기고 나중에 수동으로라도 넣어준다.
                        // log
                    }
                }
            });
        }
    }
}

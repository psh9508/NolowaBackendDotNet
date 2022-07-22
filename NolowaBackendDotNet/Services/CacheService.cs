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
    public interface ICacheService
    {
        Task SaveAndQueueToSaveDisk<T>(T data);
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ConcurrentQueue<dynamic> _queue = new ConcurrentQueue<dynamic>();
        private readonly NolowaContext _context;
        private bool _isQueueThreadStarted;


        public CacheService(IDistributedCache cache, NolowaContext context)
        {
            _cache = cache;
            _context = context;

            if (_isQueueThreadStarted == false)
                StartInsertThread();
        }

        public async Task SaveAndQueueToSaveDisk<T>(T data)
        {
            var randomId = Guid.NewGuid().ToString();

            // 캐시에 저장되면 바로 리턴
            await _cache.SetRecoredAsync(randomId, data);

            await QueueToSaveDisk(data);
        }

        private async Task QueueToSaveDisk<T>(T data)
        {
            _queue.Enqueue(data);
        }

        private void StartInsertThread()
        {
            Task.Run(async () =>    
            {
                _isQueueThreadStarted = true;

                while (true)
                {
                    try
                    {
                        bool hasSavedData = false;
                        await Task.Delay(TimeSpan.FromSeconds(10));

                        using(var context = new NolowaContext())
                        {
                            for (int i = 0; i < _queue.Count; i++)
                            {
                                if (_queue.TryDequeue(out dynamic data))
                                {
                                    // 다형성으로 해결할 수 없을까?
                                    if (data is DirectMessage)
                                        context.DirectMessages.AddRange(data);

                                    hasSavedData = true;
                                }
                            }

                            if (hasSavedData)
                                await context.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        // 이 쓰레드는 절대 죽어선 안된다.
                        // 실패 데이터는 어딘가 남기고 수동으로 넣어준다.
                        // log
                    }
                }
            });
        }
    }
}

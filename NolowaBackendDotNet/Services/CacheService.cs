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
        private static readonly ConcurrentQueue<dynamic> _queue = new ConcurrentQueue<dynamic>();
        private readonly NolowaContext _context;


        public CacheService(IDistributedCache cache, NolowaContext context)
        {
            _cache = cache;
            _context = context;

            // 생성자가 여러번 호출 됨.
            StartInsertThread();
        }

        public async Task SaveAndQueueToSaveDisk<T>(T data)
        {
            var randomId = Guid.NewGuid().ToString();

            // 캐시에 저장되면 바로 리턴
            await _cache.SetRecoredAsync(randomId, data);

            QueueToSaveDisk(data);
        }

        private void QueueToSaveDisk<T>(T data)
        {
            _queue.Enqueue(data);
        }

        private void StartInsertThread()
        {
            Task.Run(async () =>    
            {
                while (true)
                {
                    try
                    {
                        bool hasSavedData = false;
                        await Task.Delay(TimeSpan.FromSeconds(10));

                        for (int i = 0; i < _queue.Count; i++)
                        {
                            if (_queue.TryDequeue(out dynamic data))
                            {
                                // 다형성으로 해결할 수 없을까?
                                if (data is DirectMessage)
                                    _context.DirectMessages.AddRange(data);

                                hasSavedData = true;
                            }
                        }

                        if(hasSavedData)
                            await _context.SaveChangesAsync();
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

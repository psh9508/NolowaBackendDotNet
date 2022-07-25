using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.Configuration;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.CacheMonitor
{
    public interface IBackgroundCacheToDBTaskQueue
    {
        ValueTask EnqueueAsync(CacheQueueData workItem);
        ValueTask<CacheQueueData> DequeueAsync(CancellationToken cancellationToken);
        IAsyncEnumerable<CacheQueueData> DequeueAll(CancellationToken cancellationToken);
    }

    /// <summary>
    /// 멀티쓰레드 구조에 대응하기 위해 Channel을 이용해 멀티쓰레드 환경의 pub/sub 구조를 만든다.
    /// </summary>
    public class BackgroundCacheToDBTaskQueue : IBackgroundCacheToDBTaskQueue
    {
        private readonly Channel<CacheQueueData> _queue;

        public BackgroundCacheToDBTaskQueue()
        {
            _queue = Channel.CreateUnbounded<CacheQueueData>(new UnboundedChannelOptions()
            {
                SingleWriter = true,
            });
        }

        public async ValueTask EnqueueAsync(CacheQueueData workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            await _queue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<CacheQueueData> DequeueAsync(CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }

        public async IAsyncEnumerable<CacheQueueData> DequeueAll([EnumeratorCancellation]CancellationToken cancellationToken)
        {
            await foreach (var item in _queue.Reader.ReadAllAsync(cancellationToken))
            {
                yield return item;
            }
        }
    }
}

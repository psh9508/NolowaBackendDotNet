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
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.CacheMonitor
{
    public interface IBackgroundCacheToDBTaskQueue
    {
        ValueTask EnqueueBackgroundWorkItemAsync(CacheQueueData workItem);

        ValueTask<CacheQueueData> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundCacheToDBTaskQueue : IBackgroundCacheToDBTaskQueue
    {
        private readonly Channel<CacheQueueData> _queue;

        public BackgroundCacheToDBTaskQueue()
        {
            _queue = Channel.CreateUnbounded<CacheQueueData>();
        }
        
        public async ValueTask EnqueueBackgroundWorkItemAsync(CacheQueueData workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            await _queue.Writer.WriteAsync(workItem);
        }

        async ValueTask<CacheQueueData> IBackgroundCacheToDBTaskQueue.DequeueAsync(CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }
}

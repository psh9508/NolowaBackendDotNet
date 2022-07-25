using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NolowaBackendDotNet.Context;
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
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.CacheMonitor
{
   public class InsertCacheToDBMonitorLoop : BackgroundService
    {
        private readonly IBackgroundCacheToDBTaskQueue _taskQueue;
        //private readonly ICacheService _cacheService;

        public InsertCacheToDBMonitorLoop(IBackgroundCacheToDBTaskQueue taskQueue/*, ICacheService cacheService*/)
        {
            _taskQueue = taskQueue;
            //_cacheService = cacheService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var retryTime = TimeSpan.FromSeconds(10);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(retryTime);

                var cachedData = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    using (var context = new NolowaContext())
                    {
                        await context.DirectMessages.AddAsync(cachedData.Data);
                        await context.SaveChangesAsync();
                    }

                    //await _cacheService.RemoveItem(cachedData.Id);
                }
                catch (Exception ex)
                {
                   
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}

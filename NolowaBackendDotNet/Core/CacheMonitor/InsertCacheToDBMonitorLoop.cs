using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InsertCacheToDBMonitorLoop> _logger;

        public InsertCacheToDBMonitorLoop(IBackgroundCacheToDBTaskQueue taskQueue, IServiceProvider servicesProvider, ILogger<InsertCacheToDBMonitorLoop> logger)
        {
            _taskQueue = taskQueue;
            _serviceProvider = servicesProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var retryTime = TimeSpan.FromSeconds(10);

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    await Task.Delay(retryTime);

                CacheQueueData processingData = null;
                
                var cachedDatas = _taskQueue.DequeueAll(stoppingToken);

                try
                {
                    using (var context = new NolowaContext())
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var cacheService = scope.ServiceProvider.GetService<ICacheService>();

                        await foreach (var item in cachedDatas)
                        {
                            processingData = item;

                            await context.DirectMessages.AddAsync(item.Data);
                            await context.SaveChangesAsync();
                            
                            await cacheService.RemoveItem(item.Id);

                            _logger.LogDebug($"처리 완료 [{item.Data}]");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await _taskQueue.EnqueueAsync(new CacheQueueData()
                    {
                        Id = processingData.Id,
                        Data = processingData.Data,
                        InsertTryCount = processingData.InsertTryCount + 1,
                    });
                }
            //}
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}

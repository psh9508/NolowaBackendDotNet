using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Extensions;
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
            CacheQueueData processingData = null;
                
            try
            {
                using (var context = new NolowaContext())
                using (var scope = _serviceProvider.CreateScope())
                {
                    var cacheService = scope.ServiceProvider.GetService<ICacheService>();

                    await foreach (var cachedData in _taskQueue.DequeueAll(stoppingToken))
                    {
                        processingData = cachedData;

                        //await Task.Delay(1000);

                        await context.DirectMessages.AddAsync(cachedData.Data);
                        await context.SaveChangesAsync();
                            
                        await cacheService.RemoveItem(cachedData.Id);

                        _logger.LogDebug($"처리 완료 [{cachedData.Data}]");
                    }
                }
            }
            catch (Exception ex)
            {
                if(processingData.IsNotNull())
                {
                    if(processingData.InsertTryCount >= 5)
                    {
                        // Log 남기고 지움
                    }
                    else
                    {
                        await _taskQueue.EnqueueAsync(new CacheQueueData()
                        {
                            Id = processingData.Id,
                            Data = processingData.Data,
                            InsertTryCount = processingData.InsertTryCount + 1,
                        });
                    }
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}

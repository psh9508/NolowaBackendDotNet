using AutoMapper;
using Microsoft.Extensions.Logging;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Core.Base;
using NolowaBackendDotNet.Core.CacheMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services.Base
{
    public interface ICacheService
    {
        Task SaveAndQueueToSaveDisk<T>(T data);
        Task RemoveItem(string key);
    }

    public class CacheQueueData
    {
        public string Id { get; set; }
        public dynamic Data { get; set; }
        public int InsertTryCount { get; set; }
    }
}

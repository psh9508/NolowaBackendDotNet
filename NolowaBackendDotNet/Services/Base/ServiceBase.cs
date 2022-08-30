using AutoMapper;
using Microsoft.Extensions.Logging;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Core.Base;
using NolowaBackendDotNet.Core.CacheMonitor;
using NolowaBackendDotNet.Core.Test;
using NolowaBackendDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services.Base
{
    public class ServiceBase<TService> : NolowaBase<TService>
    {
        protected NolowaContext _context;
        protected IMapper _mapper;
        protected readonly IJWTTokenProvider _jwtTokenProvider;

        public ServiceBase()
        {
            if (TestHelper.IsTest)
                return;

            _context = InstanceResolver.Instance.Resolve<NolowaContext>();
            _mapper = InstanceResolver.Instance.Resolve<IMapper>();
            _jwtTokenProvider = InstanceResolver.Instance.Resolve<IJWTTokenProvider>();
        }
    }
}

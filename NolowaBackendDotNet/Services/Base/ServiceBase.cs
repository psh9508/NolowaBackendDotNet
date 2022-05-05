using AutoMapper;
using Microsoft.Extensions.Logging;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services.Base
{
    public class ServiceBase<TService> : NolowaBase<TService>
    {
        protected readonly NolowaContext _context;
        protected readonly IMapper _mapper;

        public ServiceBase()
        {
            _context = InstanceResolver.Instance.Resolve<NolowaContext>();
            _mapper = InstanceResolver.Instance.Resolve<IMapper>();
        }
    }
}

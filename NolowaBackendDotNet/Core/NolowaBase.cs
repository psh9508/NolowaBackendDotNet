using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core
{
    public class NolowaBase<TClass>
    {
        protected readonly ILogger<TClass> _logger;

        public NolowaBase()
        {
            _logger = InstanceResolver.Instance.Resolve<ILogger<TClass>>();
        }
    }
}

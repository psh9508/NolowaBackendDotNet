using Microsoft.Extensions.Logging;
using NolowaBackendDotNet.Core.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.Base
{
    public class NolowaBase<TClass>
    {
        protected readonly ILogger<TClass> _logger;

        public NolowaBase()
        {
            if (TestHelper.IsTest)
                return;

            _logger = InstanceResolver.Instance.Resolve<ILogger<TClass>>();
        }
    }
}

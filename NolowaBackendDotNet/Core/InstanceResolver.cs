// compile with: /reference:cs0433_1.dll /reference:TypeBindConflicts=Microsoft.Extensions.DependencyInjection.dll  
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core
{
    public sealed class InstanceResolver
    {
        private static readonly Lazy<InstanceResolver> _instance = new Lazy<InstanceResolver>(() => new InstanceResolver());
        public static InstanceResolver Instance { get { return _instance.Value; } }

        // 매우 지저분 하지만 많이 고민한 결과..
        private bool isSet;
        private IServiceProvider _serviceProvider;

        public IServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
            set 
            {
                if(isSet)
                    throw new InvalidOperationException("이 객체는 2번 set 될 수 없습니다.");

                _serviceProvider = value;
                isSet = true;
            }
        }

        private InstanceResolver() { }

        public T Resolve<T>() where T : class
        {
            return (T)_serviceProvider.GetRequiredService(typeof(T));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.SNSLogin.Base
{
    public enum SNSType
    {
        Google,
        Meta,
        Kakao,
    }

    public class SNSLoginBase 
    {
        protected readonly IHttpProvider _httpProvider;

        public SNSLoginBase() 
        {
            _httpProvider = InstanceResolver.Instance.Resolve<IHttpProvider>();
        }
    }
}

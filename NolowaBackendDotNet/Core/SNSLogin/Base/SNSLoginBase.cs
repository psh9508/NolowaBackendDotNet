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

        public SNSLoginBase() {  }

        public SNSLoginBase(IHttpProvider httpProvider)
        {
            _httpProvider = httpProvider;
        }
    }
}

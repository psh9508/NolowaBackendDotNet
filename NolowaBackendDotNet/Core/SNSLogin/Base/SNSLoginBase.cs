using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        protected string GetQueryString(string uri, Dictionary<string, string> values)
        {
            if (uri[^1] == '/')
                uri = uri.Remove(uri.Length - 1);

            var sb = new StringBuilder(uri);

            for (int i = 0; i < values.Count; i++)
            {
                sb.Append(i == 0 ? "?" : "&");

                sb.Append(values.Keys.ElementAt(i));
                sb.Append("=");
                sb.Append(values[values.Keys.ElementAt(i)]);
            }

            return sb.ToString();
        }
    }
}

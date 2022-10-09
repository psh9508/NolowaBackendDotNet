using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NolowaBackendDotNet.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.SNSLogin.Base
{
    public abstract class SNSLoginBase
    {
        abstract protected string AuthenticationPageURI { get; }
        abstract protected string AccessTokenURI { get; }
        abstract protected string UserInfoURI { get; }

        protected readonly IHttpProvider _httpProvider;

        //public SNSLoginBase() 
        //{
        //    _httpProvider = InstanceResolver.Instance.Resolve<IHttpProvider>();
        //}

        public SNSLoginBase(IHttpProvider httpProvider)
        {
            _httpProvider = httpProvider;
        }   

        public string GetQueryString(string uri, Dictionary<string, string> values)
        {
            // When It's ended with '/'
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

        public async Task<TResponse> GetUserInfoAsync<TResponse>()
        {
            if (_httpProvider.HasHeader("Authorization") == false)
                throw new InvalidOperationException("AccessToken을 먼저 발급 받아야 합니다.");

            var userInfoResponse = await _httpProvider.GetAsync<TResponse>(UserInfoURI);

            if(userInfoResponse.IsSuccess == false)
                return default(TResponse);

            return userInfoResponse.Body;
        }
    }
}

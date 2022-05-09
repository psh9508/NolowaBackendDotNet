using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.SNSLogin.Kakao
{
    public class KakaoLoginAccessRequest
    {
        /// <summary>
        /// Required = O
        /// authorization_code로 고정
        /// </summary>
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = "authorization_code";

        /// <summary>
        /// Required = O
        /// 앱 REST API 키
        /// </summary>
        [JsonPropertyName("client_id")]
        public string ClientID { get; set; } = string.Empty;

        /// <summary>
        /// Required = O
        /// 인가 코드가 리다이렉트된 URI
        /// </summary>
        [JsonPropertyName("redirect_uri")]
        public string RedirectUrl { get; set; } = string.Empty;

        /// <summary>
        /// Required = O
        /// 인가 코드 받기 요청으로 얻은 인가 코드
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Required = X
        /// 토큰 발급 시, 보안을 강화하기 위해 추가 확인하는 코드
        /// </summary>
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; } = string.Empty;
    }
}

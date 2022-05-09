using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.SNSLogin.Kakao
{
    public class KakaoLoginAccessResponse
    {
        /// <summary>
        /// 토큰 타입, bearer로 고정
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "bearer";

        /// <summary>
        /// 사용자 액세스 토큰 값
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 액세스 토큰과 ID 토큰의 만료 시간(초)
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 사용자 리프레시 토큰 값
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// 리프레시 토큰 만료 시간(초)
        /// </summary>
        [JsonPropertyName("refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }
    }
}

using NolowaBackendDotNet.Models.SNSLogin.Kakao.InnerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.SNSLogin.Kakao
{
    public class KakaoAccount
    {
        [JsonPropertyName("has_email")]
        public bool HasEmail { get; set; }

        /// <summary>
        /// 카카오계정 대표 이메일
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 프로필 정보
        /// </summary>
        [JsonPropertyName("profile")]
        public KakaoProfile Profile { get; set; } = new KakaoProfile();
    }
}

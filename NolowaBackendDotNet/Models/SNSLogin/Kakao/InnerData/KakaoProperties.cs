using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.SNSLogin.Kakao
{
    public class KakaoProperties
    {
        /// <summary>
        /// 닉네임
        /// </summary>
        [JsonPropertyName("nickname")]
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 프로필 사진 URL 640px* 640px 또는 480px* 480px
        /// </summary>
        [JsonPropertyName("profile_image")]
        public string ProfileImage { get; set; } = string.Empty;

        /// <summary>
        /// 프로필 미리보기 이미지 URL 110px* 110px 또는 100px* 100px
        /// </summary>
        [JsonPropertyName("thumbnail_image")]
        public string ThumbnailImage { get; set; } = string.Empty;
    }
}

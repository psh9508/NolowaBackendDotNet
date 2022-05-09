using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.SNSLogin.Kakao.InnerData
{
    public class KakaoProfile
    {
        /// <summary>
        /// 닉네임
        /// </summary>
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 프로필 미리보기 이미지 URL 110px* 110px 또는 100px* 100px
        /// </summary>
        public string ThumbnailImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 프로필 사진 URL 640px* 640px 또는 480px* 480px
        /// </summary>
        public string ProfileImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 프로필 사진 URL이 기본 프로필 사진 URL인지 여부
        /// 사용자가 등록한 프로필 사진이 없을 경우, 기본 프로필 사진 제공
        /// true: 기본 프로필 사진
        /// false: 사용자가 등록한 프로필 사진
        /// </summary>
        public bool IsDefaultImage { get; set; }
    }
}

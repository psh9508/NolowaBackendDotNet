using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.SNSLogin.Kakao
{
    public class KakaoLoginUserInfoResponse : SNSUserResponseBase
    {
        /// <summary>
        /// 회원번호
        /// </summary>
        [JsonPropertyName("id")]
        public long ID { get; set; }

        /// <summary>
        /// 자동 연결 설정을 비활성화한 경우만 존재
        /// 연결하기 호출의 완료 여부
        /// false: 연결 대기(Preregistered) 상태
        /// true: 연결(Registered) 상태
        /// </summary>
        [JsonPropertyName("has_signed_up")]
        public bool HasSignedUp { get; set; }

        /// <summary>
        /// 서비스에 연결 완료된 시각, UTC
        /// </summary>
        [JsonPropertyName("connected_at")]
        public DateTime ConnectedAt { get; set; }

        /// <summary>
        /// 카카오싱크 간편가입을 통해 로그인한 시각, UTC
        /// </summary>
        [JsonPropertyName("synched_at")]
        public DateTime SynchedAt { get; set; }

        /// <summary>
        /// 사용자 프로퍼티(Property)
        /// </summary>
        [JsonPropertyName("properties")]
        public KakaoProperties Properties { get; set; } = new KakaoProperties();

        /// <summary>
        /// 카카오계정 정보
        /// </summary>
        [JsonPropertyName("kakao_account")]
        public KakaoAccount KakaoAccount { get; set; } = new KakaoAccount();

        public override string Email { get => KakaoAccount.Email; set => KakaoAccount.Email = value; }
        public override string Name { get => KakaoAccount.Profile.NickName; set => KakaoAccount.Profile.NickName = value; }
    }
}

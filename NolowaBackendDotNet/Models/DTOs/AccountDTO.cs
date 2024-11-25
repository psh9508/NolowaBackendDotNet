using NolowaNetwork.Models.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NolowaBackendDotNet.Models.DTOs
{
    public class AccountDTO : NetMessageBase
    {
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime InsertDate { get; set; }
        public string JWTToken { get; set; } = string.Empty;
        public ProfileInfoDTO ProfileInfo { get; set; } = new ProfileInfoDTO();
        public IEnumerable<FollowerDTO> Followers { get; set; } = Enumerable.Empty<FollowerDTO>();
        public IEnumerable<PostDTO> Posts { get; set; } = Enumerable.Empty<PostDTO>();
    }
}

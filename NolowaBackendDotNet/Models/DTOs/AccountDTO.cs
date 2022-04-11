using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.DTOs
{
    public class AccountDTO
    {
        public long Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime InsertDate { get; set; }
        public string JWTToken { get; set; } = string.Empty;
        public ProfileImage ProfileImage { get; set; } = new ProfileImage();
        public IEnumerable<Follower> Followers { get; set; } = Enumerable.Empty<Follower>();
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet.Models
{
    public partial class ProfileInfo
    {
        public ProfileInfo()
        {
            Accounts = new HashSet<Account>();
        }

        public long Id { get; set; }
        public long? ProfileImgId { get; set; }
        public long? BackgroundImgId { get; set; }
        public string Message { get; set; }

        public virtual ProfileImage BackgroundImg { get; set; }
        public virtual ProfileImage ProfileImg { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }
}

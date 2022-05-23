using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet
{
    public partial class ProfileImage
    {
        public ProfileImage()
        {
            Accounts = new HashSet<Account>();
        }

        public long Id { get; set; }
        public string FileHash { get; set; }
        public string Url { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}

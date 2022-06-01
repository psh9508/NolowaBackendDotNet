using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet.Models
{
    public partial class ProfileImage
    {
        public ProfileImage()
        {
            ProfileInfoBackgroundImgs = new HashSet<ProfileInfo>();
            ProfileInfoProfileImgs = new HashSet<ProfileInfo>();
        }

        public long Id { get; set; }
        public string FileHash { get; set; }
        public string Url { get; set; }

        public virtual ICollection<ProfileInfo> ProfileInfoBackgroundImgs { get; set; }
        public virtual ICollection<ProfileInfo> ProfileInfoProfileImgs { get; set; }
    }
}

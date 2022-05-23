using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet
{
    public partial class ProfileInfo
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string BackgroundImg { get; set; }
        public string Message { get; set; }

        public virtual Account Account { get; set; }
    }
}

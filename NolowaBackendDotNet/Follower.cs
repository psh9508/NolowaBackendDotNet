using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet
{
    public partial class Follower
    {
        public long Id { get; set; }
        public long DestinationAccountId { get; set; }
        public long SourceAccountId { get; set; }

        public virtual Account DestinationAccount { get; set; }
        public virtual Account SourceAccount { get; set; }
    }
}

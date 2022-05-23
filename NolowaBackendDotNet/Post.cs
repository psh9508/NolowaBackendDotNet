using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet
{
    public partial class Post
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public DateTime? InsertDate { get; set; }
        public long AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}

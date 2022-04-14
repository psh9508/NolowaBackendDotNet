using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet.Models
{
    public partial class Post
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public DateTime? InsertDate { get; set; }
        public long AccountId { get; set; }

        [JsonProperty("postedUser")]
        public virtual Account Account { get; set; }
    }
}

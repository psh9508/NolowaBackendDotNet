using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.DTOs
{
    public class PostDTO
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public DateTime? InsertDate { get; set; }
        public long AccountId { get; set; }

        [JsonProperty("postedUser")]
        public AccountDTO Account { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models
{
    public class NolowaImage
    {
        [JsonProperty("url")]
        public string URL { get; set; } = string.Empty;

        [JsonProperty("hash")]
        public string Hash { get; set; } = "ProfilePicture";
    }
}

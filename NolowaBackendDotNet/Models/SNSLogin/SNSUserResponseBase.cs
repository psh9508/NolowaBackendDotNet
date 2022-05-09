using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.SNSLogin
{
    public class SNSUserResponseBase
    {
        [JsonProperty("email")]
        public virtual string Email { get; set; } = string.Empty;
        [JsonProperty("name")]
        public virtual string Name { get; set; } = string.Empty;
    }
}

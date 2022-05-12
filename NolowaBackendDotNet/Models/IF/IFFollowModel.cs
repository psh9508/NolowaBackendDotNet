using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.IF
{
    public class IFFollowModel
    {
        [JsonPropertyName("source_id")]
        public long SourceID { get; set; }
        [JsonPropertyName("dest_id")]
        public long DestID { get; set; }
    }
}

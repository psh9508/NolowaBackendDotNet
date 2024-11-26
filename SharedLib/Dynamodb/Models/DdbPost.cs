using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ThirdParty.Json.LitJson;

namespace SharedLib.Dynamodb.Models
{
    [DynamoDBTable("NolowaDatabase")]
    public class DdbPost : DdbBase
    {
        public override string Prefix => "p";

        public override string USN { get; set; } = string.Empty;

        public long PostId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? InsertDate { get; set; }

        //[JsonProperty("postedUser")]
        //public virtual Account Account { get; set; }
    }
}

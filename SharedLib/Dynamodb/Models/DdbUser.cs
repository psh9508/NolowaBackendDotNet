using Amazon.DynamoDBv2.DataModel;

namespace SharedLib.Dynamodb.Models
{
    [DynamoDBTable("NolowaDatabase")]
    public class DdbUser
    {
        [DynamoDBHashKey]
        public string PK { get; set; } = string.Empty;
        [DynamoDBRangeKey]
        public string SK { get; set; } = string.Empty;
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; } = new();
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        //public List<Follower> Follower { get; set; }
        //public ProfileInfo ProfileInfo { get; set; } = new();
        public string ProfileImageFile { get; set; } = string.Empty;

        [DynamoDBIgnore]
        public string Jwt { get; set; } = string.Empty;
    }
}

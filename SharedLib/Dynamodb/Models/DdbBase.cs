using Amazon.DynamoDBv2.DataModel;

namespace SharedLib.Dynamodb.Models
{
    abstract public class DdbBase
    {
        abstract public string Prefix { get; }
        abstract public string USN { get; set; }

        [DynamoDBHashKey]
        public string PK { get; set; } = string.Empty;
        [DynamoDBRangeKey]
        public string SK { get; set; } = string.Empty;

        public string GetPKString()
        {
            if(USN == string.Empty)
                throw new InvalidOperationException("USN can't be string.Empty");

            return $"{Prefix}#{USN}";
        }
    }
}

using Amazon.DynamoDBv2.DataModel;
using System.Reflection;

namespace SharedLib.Dynamodb.Models
{
    abstract public class DdbBase
    {
        [DynamoDBIgnore]
        abstract public string Prefix { get; }
        abstract public string USN { get; set; }

        // 실제로 DB에 접근할 키
        // 이 값을 실제로 쓰지 않으면 USN이 키로 들어간다.
        [DynamoDBIgnore]
        public string Key { get; set; } = string.Empty;

        [DynamoDBHashKey]
        public string PK { get; set; } = string.Empty;
        [DynamoDBRangeKey]
        public string SK { get; set; } = string.Empty;

        public string GetPKString()
        {
            if (Key == string.Empty)
                Key = USN;

            if(Key == string.Empty)
                throw new InvalidOperationException("Key can't be string.Empty");

            return $"{Prefix}#{Key}";
        }

        public string GetTableName()
        {
            var tableNameAttribute = this.GetType().GetCustomAttribute<DynamoDBTableAttribute>();

            if (tableNameAttribute is null)
                throw new InvalidOperationException("DynamoDB 접근 테이블에 DynamoDBTable가 작성되지 않았습니다.");

            return tableNameAttribute.TableName;
        }
    }
}

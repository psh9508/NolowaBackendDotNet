using NolowaNetwork.Models.Message;
using SharedLib.Converter;
using System.Text.Json.Serialization;

namespace SharedLib.Messages
{
    public class SignUpReq : NetMessageBase
    {
        public string AccountName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        //[JsonConverter(typeof(ByteArrayConverter))]
        //public byte[] ProfileImage { get; set; }
    }
}

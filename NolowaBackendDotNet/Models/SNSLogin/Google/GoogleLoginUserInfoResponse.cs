using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.SNSLogin.Google
{
    public class GoogleLoginUserInfoResponse
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("verified_email")]
        public bool VerifiedEmail { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; } = string.Empty;
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; } = string.Empty;
        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;
        [JsonPropertyName("locale")]
        public string Locale { get; set; } = string.Empty;
    }
}

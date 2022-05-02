using NolowaBackendDotNet.Core.SNSLogin.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.SNSLogin.Google
{
    public class GoogleLoginAccessRequest : ISNSRequestModel
    {
        public string URI => "https://oauth2.googleapis.com/token";

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
        [JsonPropertyName("client_id")]
        public string ClientID { get; set; } = string.Empty;
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; } = string.Empty;
        [JsonPropertyName("redirect_uri")]
        public string RedirectUrl { get; set; } = string.Empty;
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = "authorization_code";
    }
}

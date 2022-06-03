using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.DTOs
{
    public class ProfileInfoDTO
    {
        [JsonProperty("profileImg")]
        public ProfileImageDTO ProfileImage { get; set; } = new ProfileImageDTO();
        [JsonProperty("backgroundImg")]
        public ProfileImageDTO BackgroundImage { get; set; } = new ProfileImageDTO();
    }
}

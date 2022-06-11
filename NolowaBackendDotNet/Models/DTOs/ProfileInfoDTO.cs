using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.DTOs
{
    public class ProfileInfoDTO
    {
        public long Id { get; set; }
        public ProfileImageDTO ProfileImage { get; set; } = new ProfileImageDTO();
        public ProfileImageDTO BackgroundImage { get; set; } = new ProfileImageDTO();
        public string Message { get; set; } = string.Empty;
    }
}

using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class ProfileImageExtension
    {
        public static ProfileImageDTO ToDTO(this ProfileImage src)
        {
            var dto = new ProfileImageDTO()
            {
                URL = src.Url,
                Hash = src.FileHash,
            };

            return dto;
        }
    }
}

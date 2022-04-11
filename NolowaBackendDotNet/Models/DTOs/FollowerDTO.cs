using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.DTOs
{
    public class FollowerDTO
    {
        public long Id { get; set; }

        public string Email { get; set; } = string.Empty;

        //public NolowaImage ProfileImage { get; set; } = new NolowaImage();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.IF
{
    public class IFSignUpUser
    {
        public string AccountName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public byte[] ProfileImage { get; set; }
    }
}

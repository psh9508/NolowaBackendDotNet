using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.Configuration
{
    public class GoogleLoginConfiguration
    {
        public string ClientID { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public string RedirectURI { get; set; } = string.Empty;
    }
}

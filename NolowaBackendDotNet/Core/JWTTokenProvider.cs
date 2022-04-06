using Microsoft.Extensions.Options;
using NolowaBackendDotNet.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core
{
    public class JWTTokenProvider
    {
        private readonly JWT _jwtOption;

        public JWTTokenProvider(IOptions<JWT> jwtOption)
        {
            _jwtOption = jwtOption.Value;
        }
    }
}

using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class AccountExtension
    {
        public static bool SetJWTToken(this Account src)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}

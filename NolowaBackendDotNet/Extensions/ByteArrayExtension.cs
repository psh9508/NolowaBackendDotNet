using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class ByteArrayExtension
    {
        public static string ToSha256(this byte[] src)
        {
            if (src.IsNull())
                return null; // null 반환 허용

            var sha256 = SHA256.Create();

            var sha256Bytes = sha256.ComputeHash(src);

            return Convert.ToBase64String(sha256Bytes);
        }
    }
}

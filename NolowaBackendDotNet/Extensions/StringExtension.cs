using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class StringExtension
    {
        public static bool IsValid(this string src)
        {
            return !String.IsNullOrEmpty(src) && !String.IsNullOrWhiteSpace(src);
        }

        public static bool IsNotVaild(this string src)
        {
            return !IsValid(src);
        }

        public static string ToSha256(this string src)
        {
            if (src.IsNull())
                return null; // null 반환 허용

            byte[] bytes = Encoding.ASCII.GetBytes(src);
            var sha256 = SHA256.Create();

            var sha256Bytes = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(sha256Bytes);
        }
    }
}

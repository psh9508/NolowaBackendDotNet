using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class ObjectExtension
    {
        public static bool IsNull(this object src)
        {
            return src == null;
        }

        public static bool IsNotNull(this object src)
        {
            return !src.IsNull();
        }

        public static Dictionary<string, string> ToDictionary<T>(this T src)
        {
            var jsonBody = JsonSerializer.Serialize(src);

            return JsonSerializer.Deserialize<Dictionary<string, string>>(jsonBody);
        }

        public static Dictionary<string, string> ToDictionary<T>(string src)
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(src);
        }
    }
}

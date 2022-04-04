using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class JsonElementExtension
    {
        public static JsonElement SafeGetProperty(this JsonElement src, string propertyName)
        {
            try
            {
                return src.GetProperty(propertyName);
            }
            catch (Exception ex)
            {
                // log
                return default(JsonElement);
            }
        }
    }
}

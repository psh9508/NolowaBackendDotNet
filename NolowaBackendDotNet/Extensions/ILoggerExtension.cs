using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class ILoggerExtension
    {
        public static void LogStartTrace(this ILogger src, [CallerMemberName] string callerName = "")
        {
            src.LogTrace($"START [{callerName}]");
        }

        public static void LogEndTrace(this ILogger src, [CallerMemberName] string callerName = "")
        {
            src.LogTrace($"END [{callerName}]");
        }
    }
}

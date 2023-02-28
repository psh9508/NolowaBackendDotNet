using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Messages
{
    public enum MessageDestination
    {
        GATEWAY,
        SERVER,
    }

    public class MessageBase
    {
        public string Origin { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public MessageDestination Target { get; set; }
        public MessageDestination Destination { get; set; }
        public string Function { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.MessageQueue
{
    public interface IMessageEventHandler
    {
        public void HandleMessage<T>(T type, byte[] body);
    }
}

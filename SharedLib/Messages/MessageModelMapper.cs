using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Messages
{
    public class MessageModelMapper
    {
        private Dictionary<string, dynamic> _messageTypeMapper;

        public MessageModelMapper()
        {
            _messageTypeMapper = new Dictionary<string, dynamic>()
            {
                ["loginmessage"] = new LoginReq(),
            };
        }

        public dynamic GetConcreteMessageType(string name)
        {
            return _messageTypeMapper[name.ToLower()];
        }
    }
}

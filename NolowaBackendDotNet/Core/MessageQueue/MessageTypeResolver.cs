using NolowaNetwork.System;
using SharedLib.Messages;
using System;

namespace NolowaBackendDotNet.Core.MessageQueue
{
    public class MessageTypeResolver : IMessageTypeResolver
    {
        public void AddType(Type type)
        {
            throw new NotImplementedException();
        }

        public Type GetType(string typeName)
        {
            if (typeName is "LoginReq")
                return typeof(LoginReq);
            else if (typeName is "SignUpReq")
                return typeof(SignUpReq);

            return null;
        }

        public dynamic GetTypeByDynamic(string typeName)
        {
            throw new NotImplementedException();
        }
    }
}

using NolowaNetwork.System;

namespace Gateway.MessageQueue
{
    public class MessageTypeResolver : IMessageTypeResolver
    {
        public void AddType(Type type)
        {
            throw new NotImplementedException();
        }

        public Type? GetType(string typeName)
        {
            throw new NotImplementedException();
        }

        public dynamic GetTypeByDynamic(string typeName)
        {
            throw new NotImplementedException();
        }
    }
}

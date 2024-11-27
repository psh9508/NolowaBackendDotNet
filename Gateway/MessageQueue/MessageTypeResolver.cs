using NolowaNetwork.System;
using SharedLib.Dynamodb.Models;
using SharedLib.Messages;
using SharedLib.Models;

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
            if (typeName is "LoginRes")
                return typeof(LoginRes);
            else if (typeName is "AccountDTO")
                return typeof(DdbUser);
            else if (typeName is "User")
                return typeof(User);
            else if (typeName is "NewPostRes")
                return typeof(NewPostRes);

            return null;
        }

        public dynamic GetTypeByDynamic(string typeName)
        {
            throw new NotImplementedException();
        }
    }
}

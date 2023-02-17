namespace NolowaBackendDotNet.Core.MessageQueue
{
    public interface IMessageQueue
    {
        public void SendMessage<T>(T message);
    }
}

using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.MessageHandler
{
    public interface IMessageHandler<T>
    {
        public Task HandleAsync(T param);
    }
}

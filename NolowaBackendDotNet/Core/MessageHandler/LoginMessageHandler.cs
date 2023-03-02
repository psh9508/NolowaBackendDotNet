using NolowaBackendDotNet.Services;
using SharedLib.Messages;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.MessageHandler
{
    public class LoginMessageHandler : IMessageHandler<LoginMessage>
    {
        private readonly IAccountsService _accountsService;

        public LoginMessageHandler(IAccountsService accoutService)
        {
            _accountsService = accoutService;
        }

        public async Task HandleAsync(LoginMessage message)
        {
            var result = await _accountsService.LoginAsync(message.Id, message.Password);

            throw new System.NotImplementedException();
        }
    }
}

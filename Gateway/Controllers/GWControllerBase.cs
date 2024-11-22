using Microsoft.AspNetCore.Mvc;
using NolowaNetwork.System;

namespace Gateway.Controllers
{
    public class GWControllerBase : ControllerBase 
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IMessageMaker _messageMaker;

        public GWControllerBase(IMessageBroker messageBroker, IMessageMaker messageMaker)
        {
            _messageBroker = messageBroker;
            _messageMaker = messageMaker;
        }
    }
}

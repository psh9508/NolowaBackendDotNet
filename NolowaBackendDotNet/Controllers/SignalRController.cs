using Microsoft.AspNetCore.Mvc;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core.Base;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Models.IF;
using NolowaBackendDotNet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SignalRController : NolowaController
    {
        private readonly ISignalRService _signalRService;

        public SignalRController(ISignalRService signalRService)
        {
            _signalRService = signalRService;
        }

        [HttpGet("chat/dialog/{senderId}/{receiverId}")]
        public async Task<IEnumerable<DirectMessage>> GetDialogAsync(long senderId, long receiverId)
        {
            return await _signalRService.GetDialogAsync(senderId, receiverId);
        }

        [HttpGet("chat/previousDialogList/{senderId}")]
        public async Task<IEnumerable<PreviousDialogListItem>> GetPreviousDialogListAsync(long senderId)
        {
            return await _signalRService.GetPreviousDialogList(senderId);
        }

    }
}

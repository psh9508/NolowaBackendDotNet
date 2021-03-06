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
    public class DirectMessageController : NolowaController
    {
        private readonly IDirectMessageService _directMessageService;

        public DirectMessageController(IDirectMessageService directMessageService)
        {
            _directMessageService = directMessageService;
        }

        [HttpGet("chat/dialog/{senderId}/{receiverId}")]
        public async Task<IEnumerable<DirectMessage>> GetDialogAsync(long senderId, long receiverId)
        {
            return await _directMessageService.GetDialogAsync(senderId, receiverId);
        }

        [HttpGet("chat/previousDialogList/{senderId}")]
        public async Task<IEnumerable<PreviousDialogListItem>> GetPreviousDialogListAsync(long senderId)
        {
            return await _directMessageService.GetPreviousDialogList(senderId);
        }

    }
}

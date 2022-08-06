using Microsoft.AspNetCore.JsonPatch;
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

        [HttpGet("chat/unreadmessagecount/{userId}")]
        public async Task<int> GetUnreadMessageCount(long userId)
        {
            return await _directMessageService.GetUnreadMessageCountAsync(userId);
        }

        [HttpPatch("chat/dialog/readmessage")]
        public async Task<int> SetReadAllMessage([FromBody] Newtonsoft.Json.Linq.JObject jsonData)
        {
            var senderId = jsonData.Value<long>("senderId");
            var receiverId = jsonData.Value<long>("receiverId");

            return await _directMessageService.SetReadAllMessageAsync(senderId, receiverId);
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Models.IF;
using NolowaBackendDotNet.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface ISignalRService
    {
        Task<IEnumerable<DirectMessage>> GetDialogAsync(long senderId, long receiverId);
    }

    public class SignalRService : ServiceBase<SignalRService>, ISignalRService
    {
        private readonly NolowaContext context;

        public SignalRService(NolowaContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DirectMessage>> GetDialogAsync(long senderId, long receiverId)
        {
            var senderDialog = await _context.DirectMessages.Where(x => x.SenderId == senderId && x.ReceiverId == receiverId)
                                                            .OrderByDescending(x => x.InsertTime)
                                                            .Take(10)
                                                            .ToListAsync();

            var receiverDialog = await _context.DirectMessages.Where(x => x.SenderId == receiverId && x.ReceiverId == senderId)
                                                              .OrderByDescending(x => x.InsertTime)
                                                              .Take(10)
                                                              .ToListAsync();

            var senderReceiverDialog = senderDialog.Concat(receiverDialog)
                                                   .OrderByDescending(x => x.InsertTime)
                                                   .Take(10)
                                                   .AsEnumerable();

            return senderReceiverDialog.OrderBy(x => x.InsertTime);
        }
    }
}

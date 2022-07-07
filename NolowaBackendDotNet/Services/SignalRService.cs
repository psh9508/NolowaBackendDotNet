﻿using AutoMapper;
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
        Task<IEnumerable<PreviousDialogListItem>> GetPreviousDialogList(long senderId);
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

        public async Task<IEnumerable<PreviousDialogListItem>> GetPreviousDialogList(long senderId)
        {
            //SELECT *
            //FROM[Nolowa].[dbo].[DirectMessage]
            //WHERE ID IN(SELECT MAX(ID)
            //            FROM[Nolowa].[dbo].[DirectMessage]
            //            WHERE SENDER_ID = 2
            //            GROUP BY RECEIVER_ID)

            //var previousDialogList = await _context.DirectMessages.Where(x => x.SenderId == senderId || x.ReceiverId == senderId)
            //                                                      .GroupBy(x => x.ReceiverId)
            //                                                      .Select(x => new
            //                                                      {
            //                                                          Id = x.Max(x => x.Id),
            //                                                      })
            //                                                      .Join(_context.DirectMessages, x => x.Id, g => g.Id, (x, g) =>
            //                                                          new
            //                                                          {
            //                                                              ReceiverId = g.ReceiverId,
            //                                                              Message = g.Message,
            //                                                              Time = g.InsertTime,
            //                                                          }
            //                                                      ).Join(_context.Accounts, x => x.ReceiverId, a => a.Id, (x, a) =>
            //                                                          new PreviousDialogListItem()
            //                                                          {
            //                                                              Account = _mapper.Map<AccountDTO>(new Account()
            //                                                              {
            //                                                                  Id = a.Id,
            //                                                                  UserId = a.UserId,
            //                                                                  AccountName = a.AccountName,
            //                                                                  ProfileInfo = new ProfileInfo()
            //                                                                  {
            //                                                                      ProfileImg = a.ProfileInfo.ProfileImg,
            //                                                                  },
            //                                                              }),
            //                                                              Message = x.Message,
            //                                                              Time = x.Time,
            //                                                          }
            //                                                      ).ToListAsync();

            var previousDialogList = new List<PreviousDialogListItem>();

            var receivers = _context.DirectMessages.Where(x => x.SenderId == senderId)
                                                   .Select(x => x.ReceiverId)
                                                   .Distinct().ToList();

            foreach (var receiver in receivers)
            {
                var send = _context.DirectMessages.Where(x => x.SenderId == senderId && x.ReceiverId == receiver)
                                                            .GroupBy(dm => dm.ReceiverId)
                                                            .Select(x => new
                                                            {
                                                                Id = x.Max(x => x.Id),
                                                            })
                                                            .Join(_context.DirectMessages, x => x.Id, g => g.Id, (x, g) =>
                                                                new
                                                                {
                                                                    ReceiverId = g.ReceiverId,
                                                                    Message = g.Message,
                                                                    Time = g.InsertTime,
                                                                }
                                                            ).Join(_context.Accounts, x => x.ReceiverId, a => a.Id, (x, a) =>
                                                                new PreviousDialogListItem()
                                                                {
                                                                    Account = _mapper.Map<AccountDTO>(new Account()
                                                                    {
                                                                        Id = a.Id,
                                                                        UserId = a.UserId,
                                                                        AccountName = a.AccountName,
                                                                        ProfileInfo = new ProfileInfo()
                                                                        {
                                                                            ProfileImg = a.ProfileInfo.ProfileImg,
                                                                        },
                                                                    }),
                                                                    Message = x.Message,
                                                                    Time = x.Time,
                                                                }
                                                            ).ToList().SingleOrDefault();

                var received = _context.DirectMessages.Where(x => x.SenderId == receiver && x.ReceiverId == senderId)
                                                        .GroupBy(dm => dm.SenderId)
                                                        .Select(x => new
                                                        {
                                                            Id = x.Max(x => x.Id),
                                                        })
                                                        .Join(_context.DirectMessages, x => x.Id, g => g.Id, (x, g) =>
                                                            new
                                                            {
                                                                SenderId = g.SenderId,
                                                                Message = g.Message,
                                                                Time = g.InsertTime,
                                                            }
                                                        ).Join(_context.Accounts, x => x.SenderId, a => a.Id, (x, a) =>
                                                            new PreviousDialogListItem()
                                                            {
                                                                Account = _mapper.Map<AccountDTO>(new Account()
                                                                {
                                                                    Id = a.Id,
                                                                    UserId = a.UserId,
                                                                    AccountName = a.AccountName,
                                                                    ProfileInfo = new ProfileInfo()
                                                                    {
                                                                        ProfileImg = a.ProfileInfo.ProfileImg,
                                                                    },
                                                                }),
                                                                Message = x.Message,
                                                                Time = x.Time,
                                                            }
                                                        ).ToList().SingleOrDefault();

                var sendTime = send?.Time;
                var receivedTime = received?.Time;

                var addedData = sendTime.CompareTo(receivedTime) > 0 ? send : received;

                previousDialogList.Add(addedData);
            }

            return previousDialogList;
        }
    }
}

using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class AccountExtension
    {
        public static AccountDTO ToDTO(this Account src)
        {
            var dto = new AccountDTO()
            {
                AccountId = src.AccountId,
                AccountName = src.AccountName,
                Email = src.Email,
                Id = src.Id,
                InsertDate = src.InsertDate,
                ProfileImage = src.ProfileImage,
            };

            foreach (var item in src.FollowerDestinationAccounts)
            {
                dto.Followers.Add(new FollowerDTO()
                {
                    Id = item.Id,
                });
            }

            return dto;
        }

        public static bool SetJWTToken(this Account src)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}

using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class PostExtension
    {
        public static PostDTO ToDTO(this Post src)
        {
            return new PostDTO()
            {
                Account = src.Account,
                AccountId = src.AccountId,
                Id = src.Id,
                InsertDate = src.InsertDate,
                Message = src.Message,
            };
        }
    }
}

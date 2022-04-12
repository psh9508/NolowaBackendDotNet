using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class FollowerExtension
    {
        public static FollowerDTO ToDTO(this Follower src)
        {
            var dto = new FollowerDTO()
            {
                Id = src.DestinationAccount.Id,
                Email = src.DestinationAccount.Email,
            };

            return dto;
        }
    }
}

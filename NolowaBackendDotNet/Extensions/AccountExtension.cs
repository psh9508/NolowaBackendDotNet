﻿using AutoMapper;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using SharedLib.Dynamodb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class AccountExtension
    {
        public static DdbUser ToDTO(this Account src)
        {
            if (src.IsNull())
                return null;

            var mapper = InstanceResolver.Instance.Resolve<IMapper>();

            return mapper.Map<DdbUser>(src);
        }
    }
}

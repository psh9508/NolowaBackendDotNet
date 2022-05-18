using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.DTOs
{
    public class FollowerDTO
    {
        public long Id { get; set; }
        public long DestinationAccountId { get; set; }
        public long SourceAccountId { get; set; }
    }
}

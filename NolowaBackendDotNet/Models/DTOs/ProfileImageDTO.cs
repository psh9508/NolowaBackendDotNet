using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.DTOs
{
    public class ProfileImageDTO : NolowaImage
    {
        public long Id { get; set; }
        //public virtual ICollection<Account> Accounts { get; set; }
    }
}

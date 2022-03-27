using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet.Models
{
    public partial class SearchHistory
    {
        public long Id { get; set; }
        public string Keyword { get; set; }
        public DateTime? InsertDate { get; set; }
        public long? AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}

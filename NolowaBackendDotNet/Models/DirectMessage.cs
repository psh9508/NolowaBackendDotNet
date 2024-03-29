﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet.Models
{
    public partial class DirectMessage
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public string Message { get; set; }
        [JsonProperty("time")]
        public string InsertTime { get; set; }
        public bool IsRead { get; set; }
    }
}

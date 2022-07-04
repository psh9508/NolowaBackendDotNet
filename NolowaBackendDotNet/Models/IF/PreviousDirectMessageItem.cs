using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.IF
{
    public class PreviousDialogListItem
    {
        public AccountDTO Account{ get; set; }

        public string Message { get; set; } = string.Empty;

        public string Time { get; set; } = string.Empty;
    }
}

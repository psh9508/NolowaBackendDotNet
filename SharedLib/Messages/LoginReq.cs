﻿using NolowaNetwork.Models.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Messages
{
    //public class LoginMessage : MessageBase
    //{
    //    public string Id { get; set; } = string.Empty;
    //    public string Password { get; set; } = string.Empty;
    //}

    public class LoginReq : NetMessageBase
    {
        public string Id { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
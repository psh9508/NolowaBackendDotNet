﻿using NolowaNetwork.Models.Message;

namespace SharedLib.Messages
{
    public class NewPostReq : NetMessageBase
    {
        public long USN { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
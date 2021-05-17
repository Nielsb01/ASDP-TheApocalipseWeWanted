﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.DTO
{
    public class HandlerResponseDTO
    {
        public bool ReturnToSender { get; set; }
        public string ResultMessage { get; set; }
    
        public HandlerResponseDTO(bool returnToSender, string resultMessage)
        {
            ReturnToSender = returnToSender;
            ResultMessage = resultMessage;
        }
    }
}

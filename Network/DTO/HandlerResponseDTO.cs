﻿using System.Diagnostics.CodeAnalysis;
using Network.Enum;

namespace Network.DTO
{
    [ExcludeFromCodeCoverage]
    public class HandlerResponseDTO
    {
        public SendAction Action { get; set; }
        public string ResultMessage { get; set; }
    
        public HandlerResponseDTO(SendAction action, string resultMessage)
        {
            Action = action;
            ResultMessage = resultMessage;
        }

        public override bool Equals(object obj)
        {
            return obj is HandlerResponseDTO dTO &&
                   Action == dTO.Action &&
                   ResultMessage == dTO.ResultMessage;
        }
    }
}

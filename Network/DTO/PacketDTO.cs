﻿using Network.DTO;
using System.Collections.Generic;

namespace Network
{
    public class PacketDTO
    {
        public PacketHeaderDTO Header { get; set; }
        public string Payload { get; set; }
        public HandlerResponseDTO HandlerResponse { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PacketDTO dTO &&
                   EqualityComparer<PacketHeaderDTO>.Default.Equals(Header, dTO.Header) &&
                   Payload == dTO.Payload &&
                   EqualityComparer<HandlerResponseDTO>.Default.Equals(HandlerResponse, dTO.HandlerResponse);
        }
    }
}
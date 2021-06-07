﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace ASD_Game.Session.DTO
{
    [ExcludeFromCodeCoverage]
    public class HeartbeatDTO
    {
        public string clientID { get; set; }
        public bool online { get; set; }
        public DateTime time { get; set; }

        public HeartbeatDTO(string clientID)
        {
            this.clientID = clientID;
            online = true;
            time = DateTime.Now;
        }
    }
}

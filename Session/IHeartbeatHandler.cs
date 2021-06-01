﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session
{
    public interface IHeartbeatHandler
    {
        public void ReceiveHeartbeat(string clientId);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    public class SessionComponent
    {
        private string _name;
        public string Name { get => _name; set => _name = value; }

        private string _sessionId;
        public string SessionId { get => _sessionId; set => _sessionId = value; }

        public SessionComponent()
        {
            
        }

        public SessionComponent(string name)
        {
            this._name = name;
            this._sessionId = Guid.NewGuid().ToString();
        }

        public void GenerateSessionId()
        {
            _sessionId = Guid.NewGuid().ToString();
        }
    }
}

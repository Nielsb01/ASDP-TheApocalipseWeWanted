﻿using System.Collections.Generic;

namespace Session
{
    public interface ISessionHandler
    {
        public bool JoinSession(string sessionId);
        public bool CreateSession(string sessionName);
        public void RequestSessions();
        public void SendHeartbeat();
        public int GetSessionSeed();
        public List<string> GetAllClients();
    }
}
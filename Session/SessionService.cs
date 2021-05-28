﻿namespace Session
{
    public class SessionService : ISessionService
    {
        private ISessionHandler _sessionHandler;
        private readonly IGameSessionHandler _gameSessionHandler; 
        public bool inSession { get; set; }

        public bool InGame { get; set; }

        public SessionService(ISessionHandler sessionHandler, IGameSessionHandler gameSessionHandler)
        {
            _sessionHandler = sessionHandler;
            _gameSessionHandler = gameSessionHandler;
        }

        public void CreateSession(string messageValue)
        {
            inSession = _sessionHandler.CreateSession(messageValue, false, null);
        }

        public void JoinSession(string messageValue)
        {
           inSession = _sessionHandler.JoinSession(messageValue);
        }
        
        public void RequestSessions()
        {
            _sessionHandler.RequestSessions();
        }

        public void StartSession()
        {
            _gameSessionHandler.SendGameSession(_sessionHandler);
        }
    }
}
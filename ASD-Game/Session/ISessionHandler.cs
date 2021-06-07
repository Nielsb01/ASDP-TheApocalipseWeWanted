﻿using Creature.Creature.NeuralNetworking.TrainingScenario;
using System.Collections.Generic;

namespace Session
{
    public interface ISessionHandler
    {
        public TrainingScenario trainingScenario { get; set; }

        public bool JoinSession(string sessionId, string userName);

        public bool CreateSession(string sessionName, string userName);

        public void RequestSessions();

        public void SendHeartbeat();

        public int GetSessionSeed();

        public List<string[]> GetAllClients();
    }
}
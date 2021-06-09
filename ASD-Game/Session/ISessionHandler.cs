using System.Collections.Generic;
using World.Models.Characters.Algorithms.NeuralNetworking.TrainingScenario;

namespace ASD_Game.Session
{
    public interface ISessionHandler
    {
        public TrainingScenario TrainingScenario { get; set; }

        public bool JoinSession(string sessionId, string userName);
        public bool CreateSession(string sessionName, string userName, bool savedGame, string sessionId, int? seed);
        public void RequestSessions();
        public int GetSessionSeed();
        public List<string[]> GetAllClients();

        public bool GetSavedGame();

        public string GetSavedGameName();

        public bool GameStarted();

        public void SetGameStarted(bool startSessie);

        public string GameName { get; set; }
        bool NotAllowedToJoin { get; set; }
    }
}
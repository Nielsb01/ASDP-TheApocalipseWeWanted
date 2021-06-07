using System;
using System.Linq;
using Castle.Core.Internal;
using DatabaseHandler;
using DatabaseHandler.POCO;
using DatabaseHandler.Repository;
using DatabaseHandler.Services;

namespace Session
{
    public class GamesSessionService : IGamesSessionService
    {
        private readonly ISessionHandler _sessionHandler;
        private readonly IDatabaseService<ClientHistoryPOCO> _clientHistoryService;
        private readonly IDatabaseService<GamePOCO> _gamePocoService;

        public GamesSessionService(ISessionHandler sessionHandler,
            IDatabaseService<ClientHistoryPOCO> clientHistoryService, IDatabaseService<GamePOCO> gamePocoService)
        {
            _sessionHandler = sessionHandler;
            _clientHistoryService = clientHistoryService;
            _gamePocoService = gamePocoService;
        }

        public void RequestSavedGames()
        {
            var allHistory = _clientHistoryService.GetAllAsync();
            var allGames = _gamePocoService.GetAllAsync();
            allHistory.Wait();
            allGames.Wait();

            var joinedTables = from p in allGames.Result
                join pi in allHistory.Result
                    on p.PlayerGUIDHost equals pi.PlayerId
                select new
                {
                    p.PlayerGUIDHost,
                    p.GameGuid,
                    p.GameName
                };

            if (joinedTables.IsNullOrEmpty())
            {
                Console.WriteLine("There are no saved games");
            }
            else
            {
                foreach (var element in joinedTables.Select(x => new {x.GameGuid, x.GameName}))
                {
                    Console.WriteLine(element.GameGuid + " " + element.GameName);
                }
            }
        }

        public void LoadGame(string value)
        {
            var allGames = _gamePocoService.GetAllAsync();

            if (allGames.Result.Where(x => x.GameGuid == value).IsNullOrEmpty())
            {
                Console.WriteLine("Game cannot be loaded as it does not exist.");
            }
            else
            {
                var gameName = allGames.Result.Where(x => x.GameGuid == value).Select(x => x.GameName).First()
                    .ToString();
                var seed = allGames.Result.Where(x => x.GameGuid == value).Select(x => x.Seed).FirstOrDefault();
                _sessionHandler.CreateSession(gameName, true, value, seed);
            }
        }
    }
}
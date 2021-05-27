using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseHandler;
using DatabaseHandler.POCO;
using DatabaseHandler.Repository;
using DatabaseHandler.Services;
using DataTransfer.DTO.Character;
using Network;
using Network.DTO;
using Newtonsoft.Json;
using Session.DTO;
using WorldGeneration;
using WorldGeneration.Models;

namespace Session
{
    public class GameSessionHandler : IPacketHandler, IGameSessionHandler
    {
        private IClientController _clientController;
        private ISessionHandler _sessionHandler;
        private IWorldService _worldService;

        public GameSessionHandler(IClientController clientController, IWorldService worldService,
            ISessionHandler sessionHandler)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.GameSession);
            _worldService = worldService;
            _sessionHandler = sessionHandler;
        }

        public void SendGameSession(ISessionHandler sessionHandler)
        {
            StartGameDTO startGameDTO = new StartGameDTO();
            _sessionHandler = sessionHandler;
            if (_sessionHandler.GetSavedGame())
            {
                Console.WriteLine("this is a saved game.");
                startGameDTO = LoadSave(_sessionHandler.GetSavedGameName());
            }
            else
            {
                startGameDTO = SetupGameHost();
            }
            SendGameSessionDTO(startGameDTO);
        }

        private StartGameDTO LoadSave(string gameGuid)
        {
            var dbConnection = new DbConnection();

            var playerRepository = new Repository<PlayerPOCO>(dbConnection);
            var servicePlayer = new ServicesDb<PlayerPOCO>(playerRepository);
            var gameRepository = new Repository<GamePOCO>(dbConnection);
            var gameService = new ServicesDb<GamePOCO>(gameRepository);

            var allPlayers = servicePlayer.GetAllAsync();
            allPlayers.Wait();
            StartGameDTO startGameDto = new StartGameDTO();
            startGameDto.SavedPlayers = allPlayers.Result.Where(x => x.GameGuid == gameGuid).ToList();

            Dictionary<string, int[]> players = new Dictionary<string, int[]>();

            foreach (var element in startGameDto.SavedPlayers)
            {
                int[] playerPosition = new int[2];
                playerPosition[0] = element.XPosition;
                playerPosition[1] = element.YPosition;
                players.Add(element.PlayerGuid, playerPosition);
            }
            
            return startGameDto;
        }

        public StartGameDTO SetupGameHost()
        {
            var dbConnection = new DbConnection();

            var playerRepository = new Repository<PlayerPOCO>(dbConnection);
            var servicePlayer = new ServicesDb<PlayerPOCO>(playerRepository);
            var gameRepository = new Repository<GamePOCO>(dbConnection);
            var gameService = new ServicesDb<GamePOCO>(gameRepository);

            string gameGuid = Guid.NewGuid().ToString();
            var gamePOCO = new GamePOCO {GameGuid = gameGuid, PlayerGUIDHost = _clientController.GetOriginId()};
            gameService.CreateAsync(gamePOCO);

            List<string> allClients = _sessionHandler.GetAllClients();
            Dictionary<string, int[]> players = new Dictionary<string, int[]>();

            // Needs to be refactored to something random in construction; this was for testing
            int playerX = 26; // spawn position
            int playerY = 11; // spawn position
            foreach (string element in allClients)
            {
                int[] playerPosition = new int[2];
                playerPosition[0] = playerX;
                playerPosition[1] = playerY;
                players.Add(element, playerPosition);
                var tmpPlayer = new PlayerPOCO
                    {PlayerGuid = element, GameGuid = gamePOCO.GameGuid, XPosition = playerX, YPosition = playerY};
                servicePlayer.CreateAsync(tmpPlayer);

                playerX += 2; // spawn position + 2 each client
                playerY += 2; // spawn position + 2 each client
            }

            StartGameDTO startGameDTO = new StartGameDTO();
            startGameDTO.GameGuid = gameGuid;
            startGameDTO.PlayerLocations = players;

            return startGameDTO;
        }

        private void SendGameSessionDTO(StartGameDTO startGameDTO)
        {
            var payload = JsonConvert.SerializeObject(startGameDTO);
            _clientController.SendPayload(payload, PacketType.GameSession);
        }

        public HandlerResponseDTO HandlePacket(PacketDTO packet)
        {
            var startGameDTO = JsonConvert.DeserializeObject<StartGameDTO>(packet.Payload);
            HandleStartGameSession(startGameDTO);
            return new HandlerResponseDTO(SendAction.SendToClients, null);
        }

        private void HandleStartGameSession(StartGameDTO startGameDTO)
        {
            _worldService.GenerateWorld(_sessionHandler.GetSessionSeed());

            foreach (var player in startGameDTO.PlayerLocations)
            {
                if (_clientController.GetOriginId() == player.Key)
                {
                    var tmp = new DbConnection();

                    var clientHistoryRepository = new Repository<ClientHistoryPoco>(tmp);
                    var tmpClientHistory = new ServicesDb<ClientHistoryPoco>(clientHistoryRepository);

                    var tmpObject = new ClientHistoryPoco() {PlayerId = player.Key, GameId = startGameDTO.GameGuid};

                    tmpClientHistory.CreateAsync(tmpObject);

                    _worldService.AddCharacterToWorld(
                        new MapCharacterDTO(player.Value[0], player.Value[1], player.Key, startGameDTO.GameGuid,
                            CharacterSymbol.CURRENT_PLAYER), true);
                }
                else
                {
                    _worldService.AddCharacterToWorld(
                        new MapCharacterDTO(player.Value[0], player.Value[1], player.Key, startGameDTO.GameGuid,
                            CharacterSymbol.ENEMY_PLAYER), false);
                }
            }

            _worldService.DisplayWorld();
        }
    }
}
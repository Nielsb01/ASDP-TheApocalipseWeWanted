using DatabaseHandler;
using DatabaseHandler.POCO;
using DatabaseHandler.Repository;
using DatabaseHandler.Services;
using Network;
using Network.DTO;
using Newtonsoft.Json;
using Session.DTO;
using System;
using System.Collections.Generic;
using WorldGeneration;
using WorldGeneration.Models;

namespace Session
{
    public class GameSessionHandler : IPacketHandler, IGameSessionHandler
    {
        private IClientController _clientController;
        private ISessionHandler _sessionHandler;
        private IWorldService _worldService;
        private INetworkComponent _networkComponent;
        private IDatabaseService<PlayerPOCO> _playerService;
        private IDatabaseService<GamePOCO> _gameService;

        public GameSessionHandler(IClientController clientController, IWorldService worldService, ISessionHandler sessionHandler, INetworkComponent networkComponent, IDatabaseService<PlayerPOCO> playerService, IDatabaseService<GamePOCO> gameService)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.GameSession);
            _worldService = worldService;
            _sessionHandler = sessionHandler;
            _networkComponent = networkComponent;
            _playerService = playerService;
            _gameService = gameService;
        }

        // TODO: get this config from the AgentConfigurationService
        public void SendAgentConfiguration()
        {
            var agentConfigurationDto = new AgentConfigurationDTO(SessionType.SendAgentConfiguration)
            {
                PlayerId = _clientController.GetOriginId(),
                AgentConfiguration = new List<ValueTuple<string, string>>()
            };
            var payload = JsonConvert.SerializeObject(agentConfigurationDto);
            _clientController.SendPayload(payload, PacketType.Agent);
        }

        public void SendGameSession()
        {
            var StartGameDTO = SetupGameHost();
            SendGameSessionDTO(StartGameDTO);
        }

        public StartGameDTO SetupGameHost()
        {
            var gamePOCO = new GamePOCO
                {GameGuid = _clientController.SessionId, PlayerGUIDHost = _clientController.GetOriginId()};
            _gameService.CreateAsync(gamePOCO);

            List<string> allClients = _sessionHandler.GetAllClients();
            Dictionary<string, int[]> players = new Dictionary<string, int[]>();

            // Needs to be refactored to something random in construction; this was for testing
            int playerX = 26; // spawn position
            int playerY = 11; // spawn position
            foreach (string clientId in allClients)
            {
                int[] playerPosition = new int[2];
                playerPosition[0] = playerX;
                playerPosition[1] = playerY;
                players.Add(clientId, playerPosition);
                var tmpPlayer = new PlayerPOCO
                    {PlayerGuid = clientId, GameGuid = gamePOCO.GameGuid, XPosition = playerX, YPosition = playerY};
                _playerService.CreateAsync(tmpPlayer);

                playerX += 2; // spawn position + 2 each client
                playerY += 2; // spawn position + 2 each client
            }

            StartGameDTO startGameDTO = new StartGameDTO();
            startGameDTO.GameGuid = _clientController.SessionId;
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
            SendAgentConfiguration();
            _worldService.GenerateWorld(_sessionHandler.GetSessionSeed());

            // add name to players
            foreach (var player in startGameDTO.PlayerLocations)
            {
                if (_clientController.GetOriginId() == player.Key)
                {
                    // add name to players
                    _worldService.AddPlayerToWorld(
                        new WorldGeneration.Player("gerrit", player.Value[0], player.Value[1],
                            CharacterSymbol.CURRENT_PLAYER, player.Key), true);
                }
                else
                {
                    _worldService.AddPlayerToWorld(
                        new WorldGeneration.Player("arie", player.Value[0], player.Value[1],
                            CharacterSymbol.ENEMY_PLAYER, player.Key), false);
                }
            }

            // Maak 3 playerobjecten die dan een agent zijn
            if (startGameDTO.PlayerLocations.Count < 8)
            {
                
                Console.WriteLine("Not enough players. Replacing " + (8 - startGameDTO.PlayerLocations.Count) +
                                  "/8 players with agents.");

                for (var i = 0; i < (8 - startGameDTO.PlayerLocations.Count); i++)
                {
                    
                }
            }
            _worldService.DisplayWorld();
        }
    }
}
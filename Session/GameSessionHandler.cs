using System;
using System.Collections.Generic;
using DatabaseHandler;
using DatabaseHandler.Poco;
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
        
        public GameSessionHandler(IClientController clientController, IWorldService worldService, ISessionHandler sessionHandler)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.GameSession);
            _worldService = worldService;
            _sessionHandler = sessionHandler;
        }
        
        public void SendGameSession(ISessionHandler sessionHandler)
        {
            _sessionHandler = sessionHandler;
            var dto = SetupGameHost();
            SendGameSessionDTO(dto);
        }
    
        
        public StartGameDto SetupGameHost()
        {
            var dbConnection = new DbConnection();

            var playerRepository = new Repository<PlayerPoco>(dbConnection);
            var servicePlayer = new ServicesDb<PlayerPoco>(playerRepository);
            var gameRepository = new Repository<GamePoco>(dbConnection);
            var gameService = new ServicesDb<GamePoco>(gameRepository);

            string gameGuid = Guid.NewGuid().ToString();
            var gamePoco = new GamePoco {GameGuid = gameGuid, PlayerGUIDHost = _clientController.GetOriginId()};
            gameService.CreateAsync(gamePoco);

            var tmpresult = gameService.GetAllAsync();

            tmpresult.Wait();

  
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
                var tmpPlayer = new PlayerPoco
                    {PlayerGuid = element, GameGuid = gamePoco.GameGuid, XPosition = playerX, YPosition = playerY};
                servicePlayer.CreateAsync(tmpPlayer);

                playerX += 2; // spawn position + 2 each client
                playerY += 2; // spawn position + 2 each client
            }

            StartGameDto startGameDto = new StartGameDto();
            startGameDto.GameGuid = gameGuid;
            startGameDto.PlayerLocations = players;

            return startGameDto;
        }
        
        private void SendGameSessionDTO(StartGameDto startGameDto)
        {
            var payload = JsonConvert.SerializeObject(startGameDto);
            _clientController.SendPayload(payload, PacketType.GameSession);
        }
        public HandlerResponseDTO HandlePacket(PacketDTO packet)
        {
            var startGameDTO = JsonConvert.DeserializeObject<StartGameDto>(packet.Payload);
            HandleStartGameSession(startGameDTO);
            return new HandlerResponseDTO(SendAction.SendToClients, null);
        }

        private void HandleStartGameSession(StartGameDto startGameDto)
        {
            if (_clientController.IsHost())
            {
                Console.WriteLine("Ik ben de host, moet iets doen met de database");
                
            }
            
            _worldService.GenerateWorld(_sessionHandler.GetSessionSeed());

            foreach (var player in startGameDto.PlayerLocations)
            {
                if (_clientController.GetOriginId() == player.Key) 
                {
                    _worldService.AddCharacterToWorld(new MapCharacterDTO(player.Value[0], player.Value[1], player.Key, startGameDto.GameGuid, CharacterSymbol.CURRENT_PLAYER), true);
                } 
                else 
                {
                    _worldService.AddCharacterToWorld(new MapCharacterDTO(player.Value[0], player.Value[1], player.Key, startGameDto.GameGuid,CharacterSymbol.ENEMY_PLAYER), false);
                }
            }
            
            _worldService.DisplayWorld();
        }
    }
}
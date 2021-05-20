using System;
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
        public Boolean _inGame;
        public Boolean InGame { get => _inGame; set => _inGame = value; }
        public GameSessionHandler(IClientController clientController, IWorldService worldService)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.GameSession);
            _worldService = worldService;
            InGame = false;
        }
        
        public void SendGameSession(string messageValue, ISessionHandler sessionHandler)
        {
            _sessionHandler = sessionHandler;
            Console.WriteLine(messageValue);
            var dto =  _sessionHandler.SetupGameHost();
            SendGameSessionDTO(dto);
            
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
            InGame = true;
            if (_clientController.IsHost())
            {
                Console.WriteLine("Ik ben de host, moet iets doen met de database");
                
            }
            
            _worldService.GenerateWorld(6969);

            foreach (var player in startGameDto.PlayerLocations)
            {
                if (_clientController.GetOriginId() == player.Key) 
                {
                    _worldService.AddCharacterToWorld(new MapCharacterDTO(player.Value[0], player.Value[1], player.Key, CharacterSymbol.CURRENT_PLAYER), true);
                } 
                else 
                {
                    _worldService.AddCharacterToWorld(new MapCharacterDTO(player.Value[0], player.Value[1], player.Key, CharacterSymbol.ENEMY_PLAYER), false);
                }
            }
            
            _worldService.DisplayWorld();
        }
    }
}
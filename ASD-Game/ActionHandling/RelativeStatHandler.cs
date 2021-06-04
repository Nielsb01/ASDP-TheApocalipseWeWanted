using System;
using Network;
using Newtonsoft.Json;
using System.Linq;
using System.Timers;
using ActionHandling.DTO;
using DatabaseHandler.POCO;
using DatabaseHandler.Services;
using Messages;
using Network.DTO;
using WorldGeneration;
using WorldGeneration.Models.HazardousTiles;
using Timer = System.Timers.Timer;

namespace ActionHandling
{
    public class RelativeStatHandler : IRelativeStatHandler, IPacketHandler
    {
        private Player _player;

        private const int STAMINA_TIMER = 1000;
        private const int RADIATION_TIMER = 1000;

        private Timer _staminaTimer;
        private Timer _radiationTimer;

        private readonly IClientController _clientController;
        private readonly IWorldService _worldService;
        private readonly IDatabaseService<PlayerPOCO> _playerServicesDB;
        private readonly IMessageService _messageService;

        public RelativeStatHandler(IClientController clientController, IWorldService worldService, IDatabaseService<PlayerPOCO> playerServicesDb, IMessageService messageService)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.RelativeStat);
            _worldService = worldService;
            _playerServicesDB = playerServicesDb;
            _messageService = messageService;
        }
        
        public void CheckStaminaTimer()
        {
            _staminaTimer = new Timer(STAMINA_TIMER);
            _staminaTimer.AutoReset = true;
            _staminaTimer.Elapsed += StaminaEvent;
            _staminaTimer.Start();
        }
        
        public void CheckRadiationTimer()
        {
            _radiationTimer = new Timer(RADIATION_TIMER);
            _radiationTimer.AutoReset = true;
            _radiationTimer.Elapsed += RadiationEvent;
            _radiationTimer.Start();
        }
        
        private void StaminaEvent(object sender, ElapsedEventArgs e)
        {
            if (_player.Stamina < 100)
            {
                var statDto = new RelativeStatDTO();
                statDto.Stamina = 1;
                SendStat(statDto);
            }
        }
        
        private void RadiationEvent(object sender, ElapsedEventArgs e)
        {
            var tile = _worldService.GetTile(
                _worldService.GetCurrentPlayer().XPosition, 
                _worldService.GetCurrentPlayer().YPosition);
            
            if (tile is GasTile)
            {
                var statDto = new RelativeStatDTO();
            
                if (_player.RadiationLevel > 0)
                {
                    statDto.RadiationLevel = -1;
                }
                else if (_player.Health > 0)
                {
                    statDto.Health = -1;
                }
            
                SendStat(statDto);
            }
        }

        public void SendStat(RelativeStatDTO statDTO)
        {
            statDTO.Id = _clientController.GetOriginId();
            SendStatDTO(statDTO);
        }

        private void SendStatDTO(RelativeStatDTO statDTO)
        {
            var payload = JsonConvert.SerializeObject(statDTO);
            _clientController.SendPayload(payload, PacketType.RelativeStat);
        }
        
        public HandlerResponseDTO HandlePacket(PacketDTO packet)
        {
            var relativeStatDTO = JsonConvert.DeserializeObject<RelativeStatDTO>(packet.Payload);
            bool handleInDatabase = (_clientController.IsHost() && packet.Header.Target.Equals("host")) || _clientController.IsBackupHost;

            var player = _worldService.GetPlayer(relativeStatDTO.Id);
            if (player.Stamina < Player.STAMINA_MAX && relativeStatDTO.Stamina != 0)
            {
                player.AddStamina(relativeStatDTO.Stamina);
                if (relativeStatDTO.Id == _clientController.GetOriginId())
                {
                    _messageService.AddMessage("Gained stamina! S: " + _player.Stamina);
                }
                //displaystats worldservice
                InsertToDatabase(relativeStatDTO, handleInDatabase, player);
                return new HandlerResponseDTO(SendAction.SendToClients, null);
            }
            if (player.RadiationLevel > 0 && relativeStatDTO.RadiationLevel != 0)
            {
                player.AddRadiationLevel(relativeStatDTO.RadiationLevel);
                if (relativeStatDTO.Id == _clientController.GetOriginId())
                {
                    _messageService.AddMessage("Radiation damage! H: " + _player.Health + " | R: " + _player.RadiationLevel);
                }
                //displaystats worldservice
                InsertToDatabase(relativeStatDTO, handleInDatabase, player);
                return new HandlerResponseDTO(SendAction.SendToClients, null);
            }
            if (player.Health > 0 && relativeStatDTO.Health != 0)
            {
                player.AddHealth(relativeStatDTO.Health);
                if (relativeStatDTO.Id == _clientController.GetOriginId())
                {
                    _messageService.AddMessage("Radiation damage! H: " + _player.Health + " | R: " + _player.RadiationLevel);
                }
                //displaystats worldservice
                InsertToDatabase(relativeStatDTO, handleInDatabase, player);
                return new HandlerResponseDTO(SendAction.SendToClients, null);
            }
            
            return new HandlerResponseDTO(SendAction.ReturnToSender, null);
        }
    
        private void InsertToDatabase(RelativeStatDTO relativeStatDTO, bool handleInDatabase, Player player)
        {
            if (handleInDatabase)
            {
                PlayerPOCO playerPOCO = _playerServicesDB.GetAllAsync().Result.FirstOrDefault(poco => poco.PlayerGuid == player.Id && poco.GameGuid == _clientController.SessionId);
                if (relativeStatDTO.Stamina != 0)
                {
                    playerPOCO.Stamina = player.Stamina;
                } 
                else if (relativeStatDTO.RadiationLevel != 0)
                {
                    playerPOCO.RadiationLevel = player.RadiationLevel;
                } 
                else if (relativeStatDTO.Health != 0)
                {
                    playerPOCO.Health = player.Health;
                }
                _playerServicesDB.UpdateAsync(playerPOCO);
            }
        }

        public void SetCurrentPlayer(Player player)
        {
            _player = _worldService.GetCurrentPlayer();
        }
    }
}
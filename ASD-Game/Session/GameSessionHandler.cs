using System.Collections.Generic;
using ASD_Game.ActionHandling;
using ASD_Game.DatabaseHandler.POCO;
using ASD_Game.DatabaseHandler.Services;
using ASD_Game.Items;
using ASD_Game.Messages;
using ASD_Game.Network;
using ASD_Game.Network.DTO;
using ASD_Game.Network.Enum;
using ASD_Game.Session.DTO;
using ASD_Game.Session.GameConfiguration;
using ASD_Game.UserInterface;
using ASD_Game.World.Models;
using ASD_Game.World.Models.Characters;
using ASD_Game.World.Services;
using Newtonsoft.Json;

namespace ASD_Game.Session
{

    public class GameSessionHandler : IPacketHandler, IGameSessionHandler
    {
        private IClientController _clientController;
        private ISessionHandler _sessionHandler;
        private readonly IRelativeStatHandler _relativeStatHandler;
        private IWorldService _worldService;
        private IMessageService _messageService;
        private IGameConfigurationHandler _gameConfigurationHandler;
        private IScreenHandler _screenHandler;
        
        private IDatabaseService<PlayerPoco> _playerDatabaseService;
        private IDatabaseService<GamePoco> _gameDatabaseService;
        private IDatabaseService<GameConfigurationPoco> _gameConfigDatabaseService;
        private IDatabaseService<PlayerItemPoco> _playerItemDatabaseService;

        public GameSessionHandler(IClientController clientController, IWorldService worldService, ISessionHandler sessionHandler, 
            IRelativeStatHandler relativeStatHandler, IDatabaseService<PlayerPoco> playerDatabaseService,
            IDatabaseService<GamePoco> gameDatabaseService, IDatabaseService<GameConfigurationPoco> gameConfigDatabaseService, IGameConfigurationHandler gameConfigurationHandler,
            IScreenHandler screenHandler, IDatabaseService<PlayerItemPoco> playerItemDatabaseService, IMessageService messageService)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.GameSession);
            _worldService = worldService;
            _messageService = messageService;
            _sessionHandler = sessionHandler;
            _relativeStatHandler = relativeStatHandler;
            _gameConfigurationHandler = gameConfigurationHandler;
            _playerDatabaseService = playerDatabaseService;
            _gameDatabaseService = gameDatabaseService;
            _gameConfigDatabaseService = gameConfigDatabaseService;
            _screenHandler = screenHandler;
            _playerItemDatabaseService = playerItemDatabaseService;
        }

        public void SendGameSession()
        {
            StartGameDTO startGameDTO = new StartGameDTO();
            SendGameSessionDTO(startGameDTO);
        }

        private void AddItemsToPlayer( string playerId, string gameId)
        {
            PlayerItemPoco poco = new() {PlayerGUID = playerId, ItemName = ItemFactory.GetBandana().ItemName, GameGUID = gameId };
            _ = _playerItemDatabaseService.CreateAsync(poco);

            poco = new() { PlayerGUID = playerId, ItemName = ItemFactory.GetKnife().ItemName, GameGUID = gameId };
            _ = _playerItemDatabaseService.CreateAsync(poco);
        }
        
        private void SendGameSessionDTO(StartGameDTO startGameDTO)
        {
            var payload = JsonConvert.SerializeObject(startGameDTO);
            _clientController.SendPayload(payload, PacketType.GameSession);
        }

        public HandlerResponseDTO HandlePacket(PacketDTO packet)
        {
            bool handleInDatabase = (_clientController.IsHost() && packet.Header.Target.Equals("host")) || _clientController.IsBackupHost;

            _screenHandler.TransitionTo(new GameScreen());

            _worldService.GenerateWorld(_sessionHandler.GetSessionSeed());
            Player currentPlayer = AddPlayersToWorld();
            if (currentPlayer != null)
            {
                _worldService.LoadArea(currentPlayer.XPosition, currentPlayer.YPosition, 10);
            }

            _relativeStatHandler.SetCurrentPlayer(_worldService.GetCurrentPlayer());
            _relativeStatHandler.CheckStaminaTimer();
            _relativeStatHandler.CheckRadiationTimer();
            _worldService.DisplayWorld();
            _worldService.DisplayStats();
            _messageService.DisplayMessages();

            if (handleInDatabase)
            {
                InsertConfigurationIntoDatabase();
                InsertGameIntoDatabase();
                InsertPlayersIntoDatabase();
            }
            return new HandlerResponseDTO(SendAction.SendToClients, null);
        }

        private void InsertPlayersIntoDatabase()
        {
            var players = _worldService.GetPlayers();
            foreach(Player player in players)
            {
                PlayerPoco playerPoco = new PlayerPoco { PlayerGUID = player.Id, GameGUID = _clientController.SessionId, GameGUIDAndPlayerGuid = _clientController.SessionId + player.Id, XPosition = player.XPosition, YPosition = player.YPosition };
                _playerDatabaseService.CreateAsync(playerPoco);
                AddItemsToPlayer(player.Id, _clientController.SessionId);
            }
        }


        private void InsertGameIntoDatabase()
        {
            var gamePOCO = new GamePoco { GameGUID = _clientController.SessionId, PlayerGUIDHost = _clientController.GetOriginId() };
            _gameDatabaseService.CreateAsync(gamePOCO);
        }

        private void InsertConfigurationIntoDatabase()
        {
            var gameConfigurationPOCO = new GameConfigurationPoco
            {
                GameGUID = _clientController.SessionId,
                NPCDifficultyCurrent = (int)_gameConfigurationHandler.GetCurrentMonsterDifficulty(),
                NPCDifficultyNew = (int)_gameConfigurationHandler.GetNewMonsterDifficulty(),
                ItemSpawnRate = (int)_gameConfigurationHandler.GetSpawnRate()
            };
            _gameConfigDatabaseService.CreateAsync(gameConfigurationPOCO);
        }

        private Player AddPlayersToWorld()
        {
            List<string[]> allClients = _sessionHandler.GetAllClients();

            int playerX = 26;
            int playerY = 11;

            Player currentPlayer = null;
            foreach (var client in allClients)
            {
                if (_clientController.GetOriginId() == client[0])
                {
                    // add name to players
                    currentPlayer = new Player(client[1], playerX, playerY,
                        CharacterSymbol.CURRENT_PLAYER, client[0]);
                    _worldService.AddPlayerToWorld(currentPlayer, true);
                }
                else
                {
                    var playerObject = new Player(client[1], playerX, playerY, CharacterSymbol.ENEMY_PLAYER, client[0]);
                    _worldService.AddPlayerToWorld(playerObject, false);
                }
                
                playerX += 2;
                playerY += 2;
            }
            return currentPlayer;
        }
    }
}
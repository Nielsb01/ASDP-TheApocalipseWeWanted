using System;
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
using ASD_Game.Session.Helpers;
using ASD_Game.UserInterface;
using ASD_Game.World.Models.Characters;
using ASD_Game.World.Services;
using Newtonsoft.Json;
using System.Timers;
using ASD_Game.Items.Services;
using ASD_Game.World.Models.Characters.StateMachine;
using WorldGeneration.StateMachine;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ASD_Game.Session
{
    public class GameSessionHandler : IPacketHandler, IGameSessionHandler
    {
        private readonly IClientController _clientController;
        private readonly ISessionHandler _sessionHandler;
        private readonly IRelativeStatHandler _relativeStatHandler;
        private readonly IGameConfigurationHandler _gameConfigurationHandler;
        private readonly IScreenHandler _screenHandler;
        private readonly IDatabaseService<PlayerPOCO> _playerDatabaseService;
        private readonly IDatabaseService<GamePOCO> _gameDatabaseService;
        private readonly IDatabaseService<GameConfigurationPOCO> _gameConfigDatabaseService;
        private readonly IDatabaseService<PlayerItemPOCO> _playerItemDatabaseService;
        private readonly IWorldService _worldService;
        private readonly IMessageService _messageService;
        private readonly IMoveHandler _moveHandler;
        private IItemService _itemService;
        private Timer AIUpdateTimer;
        private int _brainUpdateTime = 10000;

        public GameSessionHandler(
            IClientController clientController,
            ISessionHandler sessionHandler,
            IRelativeStatHandler relativeStatHandler,
            IGameConfigurationHandler gameConfigurationHandler,
            IScreenHandler screenHandler,
            IDatabaseService<PlayerPOCO> playerDatabaseService,
            IDatabaseService<GamePOCO> gameDatabaseService,
            IDatabaseService<GameConfigurationPOCO> gameConfigDatabaseService,
            IDatabaseService<PlayerItemPOCO> playerItemDatabaseService,
            IWorldService worldService,
            IMessageService messageService,
            IItemService itemService,
            IMoveHandler moveHandler)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.GameSession);
            _sessionHandler = sessionHandler;
            _relativeStatHandler = relativeStatHandler;
            _gameConfigurationHandler = gameConfigurationHandler;
            _screenHandler = screenHandler;
            _playerDatabaseService = playerDatabaseService;
            _gameDatabaseService = gameDatabaseService;
            _gameConfigDatabaseService = gameConfigDatabaseService;
            _playerItemDatabaseService = playerItemDatabaseService;
            _worldService = worldService;
            _messageService = messageService;
            _itemService = itemService;
            _moveHandler = moveHandler;
            CheckAITimer();
            UpdateBrain();
        }

        public void SendGameSession()
        {
            StartGameDTO startGameDTO = new StartGameDTO();
            SendGameSessionDTO(startGameDTO);
        }

        private void AddItemsToPlayer(string playerId, string gameId)
        {
            PlayerItemPOCO poco = new() { PlayerGUID = playerId, ItemName = ItemFactory.GetBandana().ItemName, GameGUID = gameId };
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
            _gameConfigurationHandler.ItemService = _worldService.ItemService;
            _itemService.ChanceForItemOnTile = (int)_gameConfigurationHandler.GetItemSpawnRate();

            Player currentPlayer = AddPlayersToWorld();

            if (currentPlayer != null)
            {
                _worldService.LoadArea(currentPlayer.XPosition, currentPlayer.YPosition, 10);
            }

            _relativeStatHandler.SetCurrentPlayer(_worldService.GetCurrentPlayer());
            _relativeStatHandler.CheckStaminaTimer();
            _relativeStatHandler.CheckRadiationTimer();

            _worldService.SetAILogic();

            if (_worldService.GetPlayer(_clientController.GetOriginId()).Name.Equals("Danny"))
            {
                _worldService.GetPlayer(_clientController.GetOriginId()).Inventory.Weapon = ItemFactory.GetKatana();
                _worldService.GetPlayer(_clientController.GetOriginId()).Inventory.Armor = ItemFactory.GetTacticalVest();
                _worldService.GetPlayer(_clientController.GetOriginId()).Inventory.Armor = ItemFactory.GetGasMask();
            }

            _worldService.DisplayWorld();
            _worldService.DisplayStats();
            _messageService.DisplayMessages();

            if (handleInDatabase)
            {
                InsertConfigurationIntoDatabase();
                InsertGameIntoDatabase();
                InsertPlayersIntoDatabase();
            }
            
            if (_clientController.IsHost())
            {
                _moveHandler.CheckAITimer();
            }

            return new HandlerResponseDTO(SendAction.SendToClients, null);
        }

        private void InsertPlayersIntoDatabase()
        {
            var players = _worldService.GetAllPlayers();
            foreach (Player player in players)
            {
                PlayerPOCO playerPoco = new PlayerPOCO { PlayerGUID = player.Id, GameGUID = _clientController.SessionId, GameGUIDAndPlayerGuid = _clientController.SessionId + player.Id, XPosition = player.XPosition, YPosition = player.YPosition };
                _playerDatabaseService.CreateAsync(playerPoco);
                AddItemsToPlayer(player.Id, _clientController.SessionId);
            }
        }

        private void InsertGameIntoDatabase()
        {
            var gamePOCO = new GamePOCO { GameGUID = _clientController.SessionId, PlayerGUIDHost = _clientController.GetOriginId() };
            _gameDatabaseService.CreateAsync(gamePOCO);
        }

        private void InsertConfigurationIntoDatabase()
        {
            var gameConfigurationPOCO = new GameConfigurationPOCO
            {
                GameGUID = _clientController.SessionId,
                NPCDifficultyCurrent = (int)_gameConfigurationHandler.GetCurrentMonsterDifficulty(),
                NPCDifficultyNew = (int)_gameConfigurationHandler.GetNewMonsterDifficulty(),
                ItemSpawnRate = (int)_gameConfigurationHandler.GetItemSpawnRate()
            };
            _gameConfigDatabaseService.CreateAsync(gameConfigurationPOCO);
        }

        private Player AddPlayersToWorld()
        {
            return PlayerSpawner.SpawnPlayers(_sessionHandler.GetAllClients(), _sessionHandler.GetSessionSeed(), _worldService, _clientController);
        }

        private void CheckAITimer()
        {
            AIUpdateTimer = new Timer(_brainUpdateTime);
            AIUpdateTimer.AutoReset = true;
            AIUpdateTimer.Elapsed += CheckAITimerEvent;
            AIUpdateTimer.Start();
        }

        [ExcludeFromCodeCoverage]
        private void CheckAITimerEvent(object sender, ElapsedEventArgs e)
        {
            AIUpdateTimer.Stop();
            UpdateBrain();
            AIUpdateTimer.Start();
        }

        [ExcludeFromCodeCoverage]
        public void UpdateBrain()
        {
            if (_sessionHandler.TrainingScenario.BrainTransplant() != null)
            {
                _worldService.UpdateBrains(_sessionHandler.TrainingScenario.BrainTransplant());
            }
        }

        [ExcludeFromCodeCoverage]
        private void SetStateMachine(Monster monster)
        {
            ICharacterStateMachine CSM = new MonsterStateMachine(monster.MonsterData, null);
            monster.MonsterStateMachine = CSM;
        }

        void IGameSessionHandler.SetStateMachine(Monster monster)
        {
            throw new System.NotImplementedException();
        }
    }
}
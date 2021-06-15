﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Timers;
using Agent.Services;
using ASD_Game.DatabaseHandler.POCO;
using ASD_Game.DatabaseHandler.Services;
using ASD_Game.Network;
using ASD_Game.Network.DTO;
using ASD_Game.Network.Enum;
using ASD_Game.World.Models.Characters;
using ASD_Game.World.Models.Characters.Algorithms.Creator;
using ASD_Game.World.Services;
using Newtonsoft.Json;
using Session.DTO;
using World.Models.Characters;

namespace Creature
{
    public class AgentHandler : IAgentHandler, IPacketHandler
    {
        private readonly IWorldService _worldService;
        private readonly IClientController _clientController;
        private readonly IDatabaseService<AgentPOCO> _databaseService;
        private readonly IConfigurationService _configurationService;
        private readonly IAgentCreator _agentCreator;
        private Timer timer;
        private int timerdelay = 1000;
        private Dictionary<string, AgentAI> _agents;
        private bool _agentIsActive;

        public AgentHandler(IWorldService worldService, IClientController clientController,
            IDatabaseService<AgentPOCO> databaseService, IConfigurationService configurationService,
            IAgentCreator agentCreator)
        {
            _worldService = worldService;
            _configurationService = configurationService;
            _agentCreator = agentCreator;
            _configurationService.CreateConfiguration("agent");
            _clientController = clientController;
            _databaseService = databaseService;
            _clientController.SubscribeToPacketType(this, PacketType.Agent);
            _agents = new Dictionary<string, AgentAI>();
            CheckAITimer();
        }

        public void Replace(string playerId)
        {
            if (_worldService.GetWorld() == null) return;
            var player = _worldService.GetPlayer(playerId);

            if (player.Id == playerId)
            {
                _agents.TryGetValue(playerId, out var agent);

                if (agent == null)
                {
                    agent = _agentCreator.CreateAgent(player, _configurationService.Configuration.Settings);
                    agent.AgentStateMachine.StartStateMachine();
                    _agents.Add(player.Id, agent);
                    _agentIsActive = true;
                }
                else if (_agentIsActive)
                {
                    agent.AgentStateMachine.StopStateMachine();
                    _agentIsActive = false;
                }
                else
                {
                    agent.AgentStateMachine.CharacterData.Position = new Vector2(player.XPosition, player.YPosition);
                    agent.AgentStateMachine.StartStateMachine();
                    _agentIsActive = true;
                }
            }
            else
            {
                _agents.TryGetValue(playerId, out var agent);

                var allAgents = _databaseService.GetAllAsync();
                allAgents.Wait();

                // If player in database
                if (allAgents.Result.All(x => x.PlayerGUID != player.Id)) return;

                var agentPoco = allAgents.Result.First();

                // If agent is not activated
                if (!agentPoco.Activated || agent == null)
                {
                    var agentConfiguration = agentPoco.AgentConfiguration;

                    // Get agent from database
                    agent = _agentCreator.CreateAgent(player, agentConfiguration);
                    _agents.Add(player.Id, agent);

                    // Activate agent
                    agent.AgentStateMachine.StartStateMachine();

                    // Update database
                    agentPoco.Activated = true;
                }
                else if (!agent.AgentStateMachine.WasStarted())
                {
                    // Activate agent
                    agent.AgentStateMachine.StartStateMachine();

                    // Update database
                    agentPoco.Activated = true;
                }
                else
                {
                    // Deactivate agent
                    agent.AgentStateMachine.StopStateMachine();

                    // Update database
                    agentPoco.Activated = false;
                }

                var updateAsync = _databaseService.UpdateAsync(agentPoco);
                updateAsync.Wait();
            }
        }

        public HandlerResponseDTO HandlePacket(PacketDTO packet)
        {
            var configurationDto = JsonConvert.DeserializeObject<AgentConfigurationDTO>(packet.Payload);
            if (packet.Header.Target != "host" && !_clientController.IsBackupHost) return new HandlerResponseDTO(SendAction.Ignore, null);

            var allAgents = _databaseService.GetAllAsync();
            allAgents.Wait();
            if (allAgents.Result.Any(x => x.PlayerGUID == configurationDto.PlayerId))
            {
                UpdateAgentConfiguration(configurationDto, allAgents.Result);
            }
            else
            {
                InsertAgentConfiguration(configurationDto);
            }

            return new HandlerResponseDTO(SendAction.SendToClients, null);
        }

        private void InsertAgentConfiguration(AgentConfigurationDTO configurationDto)
        {
            var agentPoco = new AgentPOCO
            {
                PlayerGUID = configurationDto.PlayerId,
                AgentConfiguration = configurationDto.AgentConfiguration,
                GameGUID = configurationDto.GameGUID,
                Activated = configurationDto.Activated
            };
            _databaseService.CreateAsync(agentPoco);
        }

        private void UpdateAgentConfiguration(AgentConfigurationDTO agentConfigurationDto,
            IEnumerable<AgentPOCO> allAgentsResult)
        {
            var agentPoco = allAgentsResult.First(x => x.PlayerGUID == agentConfigurationDto.PlayerId);
            agentPoco.AgentConfiguration = agentConfigurationDto.AgentConfiguration;
            _databaseService.UpdateAsync(agentPoco);
        }

        private void CheckAITimer()
        {
            timer = new Timer(timerdelay)
            {
                AutoReset = true
            };
            timer.Elapsed += CheckAITimerEvent;
            timer.Start();
        }

        private void CheckAITimerEvent(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            CheckAgents();
            timer.Start();
        }

        private void CheckAgents()
        {
            if (FindAgent() != null)
            {
                FindAgent().AgentStateMachine.StopStateMachine();
            }
        }

        private AgentAI FindAgent()
        {
            List<Player> players = _worldService.GetAllPlayers();
            if (players != null)
            {
                foreach (Player player in players)
                {
                    if (player.Health <= 0)
                    {
                        if (_agents.ContainsKey(player.Id))
                        {
                            return _agents[player.Id];
                        }
                    }
                }
            }
            return null;
        }
    }
}
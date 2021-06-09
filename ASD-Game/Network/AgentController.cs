﻿using System;
using Creature.Creature;
using Network;
using Network.DTO;
using World.Models.Characters;

namespace WorldGeneration
{
    public class AgentController : IAgentController
    {
        private IClientController _clientController;
        private IWorldService _worldService;

        public AgentController(IClientController clientController, IWorldService worldService)
        {
            _worldService = worldService;
            _clientController = clientController;
            _clientController.AbsoluteOriginId = Guid.NewGuid().ToString();
            _clientController.IsBackupHost = false;
        }


        public Player CreateAgent(string sessionId)
        {
            throw new NotImplementedException();
        }

        public void HandlePacket(PacketDTO packet)
        {
            
            Console.WriteLine(packet.Payload);
            //return new(SendAction.Ignore, "result...");
        }

        HandlerResponseDTO IPacketHandler.HandlePacket(PacketDTO packet)
        {
            throw new NotImplementedException();
        }
        
        public void setSessionId(string sessionId)
        {
            _clientController.SetSessionId(sessionId);
        }
    }
}
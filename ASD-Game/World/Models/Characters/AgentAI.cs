﻿using ASD_Game.World.Models.Characters;
using ASD_Game.World.Models.Characters.StateMachine;
using ASD_Game.World.Models.Characters.StateMachine.Data;

namespace World.Models.Characters
{
    public class AgentAI : Character
    {
        public ICharacterStateMachine AgentStateMachine { get; set; }
        public AgentData AgentData { get; set; }

        public AgentAI(string name, int xPosition, int yPosition, string symbol, string id) : base(name, xPosition, yPosition, symbol, id)
        {
            SetStats(0);
            AgentStateMachine = new AgentStateMachine(AgentData);
        }

        private void SetStats(int difficulty)
        {
            CreateAgentData(difficulty);
        }

        private void CreateAgentData(int difficulty)
        {
            AgentData = new AgentData(XPosition, YPosition, difficulty);
        }
    }
}
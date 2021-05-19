﻿using Creature.Creature.StateMachine;

namespace Creature
{
    public class Monster : ICreature
    {
        private ICreatureStateMachine _monsterStateMachine;

        public ICreatureStateMachine CreatureStateMachine
        {
            get => _monsterStateMachine;
        }

        public Monster(ICreatureStateMachine monsterStateMachine)
        {
            _monsterStateMachine = monsterStateMachine;
            _monsterStateMachine.StartStateMachine();
        }

        public void ApplyDamage(double amount)
        {
            _monsterStateMachine.CreatureData.Health -= amount;
        }

        public void HealAmount(double amount)
        {
            _monsterStateMachine.CreatureData.Health += amount;
        }
        
        public void SendChatMessenge(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}

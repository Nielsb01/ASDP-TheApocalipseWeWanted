﻿using Creature.Creature;

namespace Creature
{
    public class UseConsumableState : ICreatureStateInterface
    {
        private Monster.Event _stateName;
        public Monster.Event StateName { get => _stateName; }
        public void Do()
        {

        }
    }
}
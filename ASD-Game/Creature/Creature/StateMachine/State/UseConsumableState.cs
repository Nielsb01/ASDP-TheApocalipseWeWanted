﻿using System;
using ASD_project.Creature.Creature.StateMachine.Data;

namespace ASD_project.Creature.Creature.StateMachine.State
{
    public class UseConsumableState : CreatureState
    {
        public UseConsumableState(ICreatureData creatureData) : base(creatureData)
        {
            _creatureData = creatureData;
        }

        public override void Do()
        {
            throw new NotImplementedException();
        }
        public override void Do(ICreatureData creatureData)
        {
            throw new NotImplementedException();
        }
    }
}
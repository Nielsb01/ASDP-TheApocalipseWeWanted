﻿

using System;
using Creature.Creature.StateMachine.Data;

namespace Creature.Creature.StateMachine.State
{
    public class InventoryState : CreatureState
    {
        public InventoryState(ICreatureData creatureData)
        {
            _creatureData = creatureData;
        }

        public override void Do()
        {
            //TODO Implement state functions.
            throw new NotImplementedException();
        }
    }
}
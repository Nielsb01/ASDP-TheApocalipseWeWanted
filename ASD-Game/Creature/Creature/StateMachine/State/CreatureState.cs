﻿using Creature.Creature.StateMachine.Data;
using System;

namespace Creature.Creature.StateMachine.State
{
    public abstract class CreatureState : IComparable
    {
        protected ICreatureData _creatureData;
        protected ICreatureData _target;
        
        public CreatureState(ICreatureData creatureData)
        {
            _creatureData = creatureData;
        }

        public virtual void Do()
        {
            throw new NotImplementedException();
        }

        public virtual void Do(ICreatureData creatureData)
        {
            throw new NotImplementedException();
        }
        
        public virtual void SetTargetData(ICreatureData data)
        {
            _target = data;
        }
        
        public int CompareTo(object obj)
        {
            throw new InvalidOperationException("State machine is not a comparable object.");
        }
    }
}
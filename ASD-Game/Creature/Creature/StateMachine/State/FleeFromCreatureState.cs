﻿using Creature.Creature.StateMachine.Data;
using System;
using System.Numerics;
using ActionHandling;
using Network;
using WorldGeneration;

namespace Creature.Creature.StateMachine.State
{
    public class FleeFromCreatureState : CreatureState
    {
        private ICreatureData _target;
        private MoveHandler _moveHandler = new MoveHandler(new ClientController(new NetworkComponent()), new WorldService());

        public FleeFromCreatureState(ICreatureData creatureData, ICreatureStateMachine stateMachine) : base(creatureData, stateMachine)
        {
            _creatureData = creatureData;
            _stateMachine = stateMachine;
        }

        public override void Do()
        {
            String direction = "";
            if (Vector2.DistanceSquared(_creatureData.Position, _target.Position) <= Vector2.DistanceSquared(new Vector2(_creatureData.Position.X + 1, _creatureData.Position.Y + 1), _target.Position))
            {
                direction = "up";
            }
            else if (Vector2.DistanceSquared(_creatureData.Position, _target.Position) <= Vector2.DistanceSquared(new Vector2(_creatureData.Position.X - 1, _creatureData.Position.Y - 1), _target.Position))
            {
                direction = "down";
            }
            _moveHandler.SendMove(direction, 1);
        }

        public void setTarget(ICreatureData target)
        {
            _target = target;
        }
       
    }
}
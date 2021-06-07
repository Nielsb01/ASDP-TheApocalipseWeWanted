﻿using System;
using Creature.Creature.StateMachine.Data;
using Creature.Pathfinder;
using System.Collections.Generic;
using System.Numerics;
using ASD_project.Creature.Creature.StateMachine.Builder;
using Creature.Creature.StateMachine.Builder;

namespace Creature.Creature.StateMachine.State
{
    public class FollowCreatureState : CreatureState
    {
        public FollowCreatureState(ICreatureData creatureData, ICreatureStateMachine stateMachine, List<BuilderInfo> builderInfoList, BuilderConfigurator builderConfiguration) : base(creatureData, stateMachine, builderInfoList, builderConfiguration)
        {
            _creatureData = creatureData;
            _stateMachine = stateMachine;
            _builderConfiguration = builderConfiguration;
            _builderInfoList = builderInfoList;
        }
        
        public FollowCreatureState(ICreatureData creatureData, ICreatureStateMachine stateMachine) : base (creatureData, stateMachine)
        {
            _creatureData = creatureData;
            _stateMachine = stateMachine;
        }
        public override void Do()
        {
            foreach (var builderInfo in _builderInfoList)
            {
            if (builderInfo.Action == "attack")
            {
                if (_builderConfiguration.GetGuard(_creatureData, _target, builderInfo.RuleSets, "follow"))
                {
                    //TODO implement Attack logic + gather targetData
                    // PathFinder pathFinder = new PathFinder(_creatureData.World.Nodes);
                    // ICreatureData playerData = creatureData;
                    //
                    // Stack<Node> newPath = pathFinder.FindPath(_creatureData.Position, playerData.Position);
                    //
                    // if (!(newPath.Peek().Position.X == playerData.Position.X && newPath.Peek().Position.Y == playerData.Position.Y))
                    // {
                    //     float newPositionX = newPath.Peek().Position.X;
                    //     float newPositionY = newPath.Peek().Position.Y;
                    //     _creatureData.Position = new Vector2(newPositionX, newPositionY);
                    //}
                }
            }
        }
    }
}
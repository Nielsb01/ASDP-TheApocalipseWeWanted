﻿using ASD_project.Creature.Creature.StateMachine.Builder;
using Creature.Creature.StateMachine.Builder;
using Creature.Creature.StateMachine.Data;
using System;
using System.Collections.Generic;

namespace Creature.Creature.StateMachine.State
{
    public class AttackState : CreatureState
    {
        public AttackState(ICreatureData creatureData, ICreatureStateMachine stateMachine) : base(creatureData, stateMachine)
        {
            _creatureData = creatureData;
            _stateMachine = stateMachine;
        }

        public override void Do()
        {
            BuilderConfiguration builderConfiguration = //haal op
            List<BuilderInfo> builderInfoList = //haal op
            foreach (var builderInfo in builderInfoList)
            {
                if (builderInfo.Action == "attack")
                {
                    if (builderConfiguration.GetGuard(_creatureData, targetData, builderInfo.RuleSets, "attack"))
                    {
                        // voer de rest van deze methode uit
                    }
                }
            }
            //TODO implement logic
            throw new NotImplementedException();
        }
    }
}
﻿using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creature
{
    public class Monster : ICreature
    {
        public enum Event { LOST_PLAYER, SPOTTED_PLAYER, ALMOST_DEAD, REGAINED_HEALTH_PLAYER_OUT_OF_RANGE, REGAINED_HEALTH_PLAYER_IN_RANGE, PLAYER_OUT_OF_RANGE, PLAYER_IN_RANGE };
        public enum State { WANDERING, FOLLOW_PLAYER, ATTACK_PLAYER, USE_POTION };

        private PassiveStateMachine<State, Event> stateMachine;

        public Monster()
        {
            startStateMachine();
        }

        public void FireEvent(Enum creatureEvent, object argument)
        {
            if (creatureEvent.GetType() == typeof(Event))
            {
                stateMachine.Fire((Event) creatureEvent, argument);
            }
        }

        public void FireEvent(Enum creatureEvent)
        {
            if (creatureEvent.GetType() == typeof(Event))
            {
                stateMachine.Fire((Event) creatureEvent);
            }
        }

        private void startStateMachine()
        {
            var builder = new StateMachineDefinitionBuilder<State, Event>();

            // Wandering
            builder.In(State.FOLLOW_PLAYER).On(Event.LOST_PLAYER).Goto(State.WANDERING);

            // Follow player
            builder.In(State.WANDERING).On(Event.SPOTTED_PLAYER).Goto(State.FOLLOW_PLAYER).Execute(() => Console.WriteLine("Now following player"));
            builder.In(State.USE_POTION).On(Event.REGAINED_HEALTH_PLAYER_OUT_OF_RANGE).Goto(State.FOLLOW_PLAYER);
            builder.In(State.ATTACK_PLAYER).On(Event.PLAYER_OUT_OF_RANGE).Goto(State.FOLLOW_PLAYER);

            // Attack player
            builder.In(State.FOLLOW_PLAYER).On(Event.PLAYER_IN_RANGE).Goto(State.ATTACK_PLAYER);
            builder.In(State.USE_POTION).On(Event.REGAINED_HEALTH_PLAYER_IN_RANGE).Goto(State.ATTACK_PLAYER);

            // Use potion
            builder.In(State.ATTACK_PLAYER).On(Event.ALMOST_DEAD).Goto(State.USE_POTION);
            builder.In(State.FOLLOW_PLAYER).On(Event.ALMOST_DEAD).Goto(State.USE_POTION);

            builder.WithInitialState(State.WANDERING);

            stateMachine = builder.Build().CreatePassiveStateMachine();
            stateMachine.Start();
        }
    }    
}

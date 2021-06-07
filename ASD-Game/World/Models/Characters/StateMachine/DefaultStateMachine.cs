﻿using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Machine;
using System;
using System.Threading;
using WorldGeneration.StateMachine.CustomRuleSet;
using WorldGeneration.StateMachine.Data;
using WorldGeneration.StateMachine.Event;
using WorldGeneration.StateMachine.State;

namespace WorldGeneration.StateMachine
{
    public abstract class DefaultStateMachine : ICharacterStateMachine
    {
        protected RuleSet _ruleset;
        protected PassiveStateMachine<CharacterState, CharacterEvent.Event> _passiveStateMachine;
        protected ICharacterData _characterData;

        public ICharacterData CharacterData
        {
            get => _characterData;
            set => _characterData = value;
        }

        protected Timer _timer;

        public DefaultStateMachine(ICharacterData characterData, RuleSet ruleset)
        {
            _characterData = characterData;
            _ruleset = ruleset;
        }

        public virtual void StartStateMachine()
        {
            _passiveStateMachine.Start();
        }

        protected void Update()
        {
            _timer = new Timer((e) =>
            {
                FireEvent(CharacterEvent.Event.DO);
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000));
        }

        protected void KillLoop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void FireEvent(CharacterEvent.Event creatureEvent, object argument)
        {
            _passiveStateMachine.Fire(creatureEvent, argument);
        }

        public void FireEvent(CharacterEvent.Event creatureEvent)
        {
            _passiveStateMachine.Fire(creatureEvent);
        }

        protected void DefineDefaultBehaviour(
            ref StateMachineDefinitionBuilder<CharacterState, CharacterEvent.Event> builder, ref CharacterState state)
        {
            builder.In(state).ExecuteOnEntry(state.Entry);
            builder.In(state).ExecuteOnExit(state.Exit);
            builder.In(state).On(CharacterEvent.Event.DO).Execute(state.Do);
        }
    }
}
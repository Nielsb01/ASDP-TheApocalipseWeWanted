﻿using System;
using WorldGeneration.StateMachine.Data;

namespace WorldGeneration.StateMachine.State
{
    public class UseConsumableState : CharacterState
    {
        public UseConsumableState(ICharacterData characterData) : base(characterData)
        {
            _characterData = characterData;
        }

        public override void Entry()
        {
            Console.WriteLine("UseConsumable state Entry");
        }

        public override void Do()
        {
            throw new NotImplementedException();
        }

        public override void Do(ICharacterData characterData)
        {
            throw new NotImplementedException();
        }

        public override void Exit()
        {
            Console.WriteLine("UseConsumable state Exit");
        }
    }
}
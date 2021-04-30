﻿using Creature.Creature.StateMachine.State;
using Creature.World;
using System.Numerics;

namespace Creature.Creature.StateMachine.Data
{
    public class MonsterData : ICreatureData
    {
        private bool _alive;
        private Vector2 _position;
        private int _health;
        private int _damage;
        private int _visionRange;
        private IWorld _world;
        private bool _following;

        public bool IsAlive
        {
            get => _alive;
            set => _alive = value;
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }
        
        public int Health
        {
            get => _health;
            set => _health = value;
        }

        public int Damage
        {
            get => _damage;
            set => _damage = value;
        }

        public int VisionRange
        {
            get => _visionRange;
            set => _visionRange = value;
        }

        public IWorld World
        {
            get => _world;
            set => _world = value;
        }

        public bool IsFollowing
        {
            get => _following;
            set => _following = value;
        }

        public MonsterData(bool isAlive, Vector2 position, int health, int damage, int visionRange, IWorld world, bool following)
        {
            _alive = isAlive;
            _position = position;
            _health = health;
            _damage = damage;
            _visionRange = visionRange;
            _world = world;
            _following = following;
        }
    }
}
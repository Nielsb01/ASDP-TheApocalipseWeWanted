﻿namespace Creature.Creature.StateMachine.CreatureData
{
    public interface ICreatureData
    {
        bool IsAlive { get; set; }
        System.Numerics.Vector2 Position { get; set; }
        int VisionRange { get; set; }
        int Damage { get; set; }
        int Health { get; set; }

    }
}

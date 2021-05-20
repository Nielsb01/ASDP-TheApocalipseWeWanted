﻿using Player.Model.ItemStats;

namespace Weapon
{
    public class Glock : Weapon
    {
        public Glock()
        {
            Name = "Glock";
            Type = WeaponType.Range;
            Rarity = Rarity.Uncommon;
            Speed = WeaponSpeed.Slow;
            Distance = WeaponDistance.Medium;
            Damage = WeaponDamage.Low;
        }
    }
}
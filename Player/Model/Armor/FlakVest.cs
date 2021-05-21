﻿
using Player.Model.Armor.ArmorStats;
using Player.Model.ItemStats;

namespace Player.Model.Armor
{
    public class FlakVest : Armor
    {
        public FlakVest()
        {
            ItemName = "Flak vest";
            ArmorType = ArmorType.Body;
            Rarity = Rarity.Uncommon;
            ArmorProtectionPoints = 20;
        }
    }
}
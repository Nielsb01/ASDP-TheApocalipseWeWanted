﻿/*
    AIM SD ASD 2020/2021 S2 project
     
    Project name: [to be determined].
 
    This file is created by team: 3.
     
    Goal of this file: Supplies interface for LootableTile super-class
     
*/

namespace WorldGeneration.Tiles.Interfaces
{
    interface ILootableTile
    {
        int GenerateLoot();
        void LootItem(int Item);
    }
}

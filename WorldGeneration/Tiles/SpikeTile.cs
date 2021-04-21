﻿/*
    AIM SD ASD 2020/2021 S2 project
     
    Project name: [to be determined].
 
    This file is created by team: 3.
     
    Goal of this file: Data object for tile properties.
     
*/

using System;

namespace WorldGeneration.Tiles
{
    public class SpikeTile : HazardousTile
    {
        public SpikeTile() 
        {
            Symbol = "^";
            Accessible = true;
            Damage = new Random().Next(2, 11);
        }

        public override int GetDamage(int Time)
        {
            return Damage;
        }
    }
}

﻿/*
    AIM SD ASD 2020/2021 S2 project
     
    Project name: [to be determined].
 
    This file is created by team: 3.
     
    Goal of this file: Data object for tile properties.
     
*/

namespace WorldGeneration.Tiles
{
    class WaterTile : Tile
    {
        public WaterTile(int X, int Y) : base(X, Y)
        {
            Symbol = "~";
            Accessible = false;
        }
    }
}

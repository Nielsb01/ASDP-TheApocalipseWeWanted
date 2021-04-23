﻿namespace WorldGeneration.Models.TerrainTiles
{
    public class StreetTile : TerrainTile
    {
        public StreetTile()
        {
            Symbol = TileSymbol.STREET;
            IsAccessible = true;
        }
    }
}
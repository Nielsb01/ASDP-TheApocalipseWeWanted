﻿using System;
using DataTransfer.Model.World.Interfaces;

namespace DataTransfer.Model.World.HazardousTiles
{
    public class SpikeTile : IHazardousTile
    {
        public bool IsAccessible { get; set; }
        public string Symbol { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public SpikeTile(int x, int y)
        {
            Symbol = TileSymbol.SPIKE;
            IsAccessible = true;
            XPosition = x;
            YPosition = y;
            Damage = new Random().Next(2, 11);
        }

        public int Damage { get; set; }

        public int GetDamage(int time)
        {
            return Damage;
        }
    }
}
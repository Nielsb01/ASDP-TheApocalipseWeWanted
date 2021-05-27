﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGeneration
{
    public class Player : Character
    {
        public string Guid { get; set; }
        public int Stamina { get; set; }
        public Inventory Inventory { get; set; }
        public int RadiationLevel { get; set; }
        public int Team { get; set; }

        //random default values for health&stamina for now
        private const int HEALTHCAP = 100;
        private const int STAMINACAP = 10;

        public Player(string name, int xPosition, int yPosition, string symbol, string guid) : base(name, xPosition, yPosition, symbol)
        {
            Guid = guid;
            Stamina = STAMINACAP;
            Health = HEALTHCAP;
            Inventory = new();
            RadiationLevel = 0;
            Team = 0;
        }
    }
}

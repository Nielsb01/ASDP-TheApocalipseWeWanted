﻿/*
    AIM SD ASD 2020/2021 S2 project
     
    Project name: ASD-Project.
 
    This file is created by team: 2.
     
    Goal of this file: Keeping track of radiation levels.

*/

namespace Player
{
    public class RadiationLevel : IRadiationLevel
    {
        private int _level;

        public RadiationLevel(int radiationLevel)
        {
            _level = radiationLevel;
        }

        public int getRadiationLevel()
        {
            return _level;
        }

        public void setRadiationLevel(int radiationLevel)
        {
            _level = radiationLevel;
        }
    }
}

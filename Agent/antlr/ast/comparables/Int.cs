﻿namespace Agent.antlr.ast.comparables
{
    /*
    AIM SD ASD 2020/2021 S2 project
     
    Project name: [to be determined].
 
    This file is created by team: 1].
     
    Goal of this file: [making_the_system_work].
         
    */
    
    
    public class Int : Comparable
    {
        public int Value { get; set; }
        
        public Int(int value)
        {
            Value = value;
        }

        public new string GetNodeType()
        {
            return "Int";
        }
    }
}
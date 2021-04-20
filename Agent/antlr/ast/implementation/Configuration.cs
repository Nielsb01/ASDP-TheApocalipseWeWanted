﻿using System.Collections;
using Agent.antlr.ast.implementation;
using Agent.antlr.ast.interfaces;

namespace Agent.antlr.ast.implementation
{
    /*
    AIM SD ASD 2020/2021 S2 project
     
    Project name: [to be determined].

    This file is created by team: 1.
     
    Goal of this file: [making_the_system_work].
     
    */
    public class Configuration : Node, IConfiguration
    {
        public Configuration(){}
        
        public Configuration(ArrayList body)
        {
            this.body = body;
        }

        public new string GetNodeType()
        {
            return "Configuration";
        }
    }
}
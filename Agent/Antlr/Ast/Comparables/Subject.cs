﻿
namespace Agent.Antlr.Ast.Comparables
{
    
    public class Subject : Comparable
    {
        public string Name { get; }
        
        public Subject(string name)
        {
            Name = name;
        }


        public override string GetNodeType()
        {
            return "Subject";
        }
    }
}
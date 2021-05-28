﻿namespace Agent.Antlr.Ast.Comparables
{
    public class Int : Comparable
    {
        public int Value { get; }
        
        public Int(int value)
        {
            Value = value;
        }

        public override string GetNodeType()
        {
            return "Int";
        }
    }
}
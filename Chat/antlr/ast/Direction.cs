﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Chat.antlr.ast
{
    public class Direction : ASTNode, IEquatable<Direction>
    {
        private string value;
        
        public Direction(string value)
        {
            this.value = value;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Direction);
        }

        [ExcludeFromCodeCoverage]
        public bool Equals(Direction other)
        {
            if (other == null)
                return false;

            return value == other.value;
        }
    }
}
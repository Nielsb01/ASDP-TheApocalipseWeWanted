﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Chat.antlr.ast
{
    public class AST : IEquatable<AST>
    {
        public Input root;

        public AST()
        {
            root = new Input();
        }

        public AST(Input input)
        {
            root = input;
        }

        public void setRoot(Input input)
        {
            root = input;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            return this.Equals(obj as AST);
        }

        [ExcludeFromCodeCoverage]
        public bool Equals(AST other)
        {
            if (other == null)
                return false;

            return root.Equals(other.root);
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            return Equals(obj as AST);
        }
    }
}
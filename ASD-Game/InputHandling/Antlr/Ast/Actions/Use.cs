﻿using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace InputHandling.Antlr.Ast.Actions
{
    public class Use : Command, IEquatable<Use>
    {
        private Step _step;
        public Step Step { get => _step; private set => _step = value; }

        public Use()
        {

        }

        public ArrayList GetChildren()
        {
            var children = new ArrayList();
            children.Add(_step);
            return children;
        }

        public override ASTNode AddChild(ASTNode child)
        {
            if (child is Step)
            {
                _step = (Step)child;
            }
            return this;
        }

        public ASTNode RemoveChild(ASTNode child)
        {
            if (child is Step && child == _step)
            {
                _step = null;
            }
            return this;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            return Equals(obj as Use);
        }

        [ExcludeFromCodeCoverage]
        public bool Equals(Use other)
        {
            if (other == null)
            {
                return false;
            }
            return _step.Equals(other.Step);
        }
    }
}

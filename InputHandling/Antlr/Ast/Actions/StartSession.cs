﻿using System.Collections;

namespace InputHandling.Antlr.Ast.Actions
{
    public class StartSession : Command
    {
        private Message _message;
        public Message Message { get => _message; private set => _message = value; }

        public ArrayList GetChildren()
        {
            var children = new ArrayList();
            children.Add(_message);
            return children;
        }

        public override ASTNode AddChild(ASTNode child)
        {
            if (child is Message)
            {
                _message = (Message)child;
            }

            return this;
        }

        public ASTNode RemoveChild(ASTNode child)
        {
            if (child is Message && child == _message)
            {
                _message = null;
            }

            return this;
        }
    }
}
﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using InputCommandHandler.Antlr.Ast;
using InputCommandHandler.Antlr.Grammar;
using InputCommandHandler.Antlr.Parser;
using InputCommandHandler.Antlr.Transformer;
using InputCommandHandler.Exceptions;
using Player.Services;
using Session;

namespace InputCommandHandler.Antlr
{
    public class Pipeline : IAntlrErrorListener<IToken>
    {
        private AST _ast;
        public AST Ast { get => _ast; private set => _ast = value; }

        public void SyntaxError(IRecognizer recognizer, 
                                IToken offendingSymbol, 
                                int line, 
                                int charPositionInLine, 
                                string msg, 
                                RecognitionException e)
        {
            throw new CommandSyntaxException(msg);
        }

        public void ParseCommand(string input)
        {
            //Lex (with Antlr's generated lexer)
            if (!input.StartsWith("say") && !input.StartsWith("whisper") && !input.StartsWith("shout"))
            {
                input = input.ToLower();
            }

            var inputStream = new AntlrInputStream(input);
            var lexer = new PlayerCommandsLexer(inputStream);
            lexer.RemoveErrorListeners();

            var tokens = new CommonTokenStream(lexer);

            //Parse (with Antlr's generated parser)
            var parser = new PlayerCommandsParser(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(this);

            var parseTree = parser.input();

            //Extract AST from the Antlr parse tree
            var listener = new ASTListener();
            var walker = new ParseTreeWalker();
            walker.Walk(listener, parseTree);

            _ast = listener.getAST();
        }
        public void Transform(IPlayerService playerService, ISessionService sessionService)
        {
            if (_ast == null)
            {
                return;
            }
            new Evaluator(playerService, sessionService).Apply(_ast);
        }
    }
}
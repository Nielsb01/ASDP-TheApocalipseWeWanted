﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using InputHandling.Antlr;
using InputHandling.Exceptions;
using WebSocketSharp;
using InputCommandHandler.Models;
using Session;
using UserInterface;
using System.Text;
using InputHandling.Models;

namespace InputHandling
{
    public class InputHandler : IInputHandler
    {
        private IPipeline _pipeline;
        private ISessionHandler _sessionHandler;
        private IScreenHandler _screenHandler;
        private static Timer  _timer;
        private const string RETURN_KEYWORD = "return";

        public InputHandler(IPipeline pipeline, ISessionHandler sessionHandler, IScreenHandler screenHandler)
        {
            _pipeline = pipeline;
            _sessionHandler = sessionHandler;
            _screenHandler = screenHandler;
        }

        public InputHandler()
        {

        }

        public void HandleGameScreenCommands()
        {
            SendCommand(GetCommand());
        }
        private void SendCommand(string commando)
        {
            try
            {
                _pipeline.ParseCommand(commando);
                _pipeline.Transform();
            }
            catch (CommandSyntaxException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (MoveException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public virtual string GetCommand()
        {
            return Console.ReadLine();
        }
        
        public void HandleStartScreenCommands()
        {
            var input = GetCommand();
            var option = 0;
            int.TryParse(input, out option);
            
            switch (option)
            {
                case 1:
                    _screenHandler.TransitionTo(new ConfigurationScreen());
                    break;
                case 2:
                    _sessionHandler.RequestSessions();
                    _screenHandler.TransitionTo(new SessionScreen());
                    break;
                case 3:
                    _screenHandler.TransitionTo(new LoadScreen());
                    break;
                case 4:
                    _screenHandler.TransitionTo(new EditorScreen());
                    break;
                case 5:
                    Environment.Exit(0);
                    break;
                default:
                    StartScreen startScreen = _screenHandler.Screen as StartScreen;
                    startScreen.UpdateInputMessage("Not a valid option, try again!");
                    break;
            }
        }

        public void HandleSessionScreenCommands()
        {
            var input = GetCommand();
            int sessionNumber = 0;
            int.TryParse(input, out sessionNumber);
            SessionScreen sessionScreen = _screenHandler.Screen as SessionScreen;

            if (input == RETURN_KEYWORD)
            {
                _screenHandler.TransitionTo(new StartScreen());
            }
            else if (sessionNumber > 0)
            {
                string sessionId = sessionScreen.GetSessionIdByVisualNumber(sessionNumber - 1);

                if (sessionId.IsNullOrEmpty())
                {
                    sessionScreen.UpdateInputMessage("Not a valid session, try again!");
                }
                else
                {
                    SendCommand("join_session \"" + sessionId + "\"");
                    _screenHandler.TransitionTo(new ConfigurationScreen()); // maybe a waiting screen in stead?   
                }
            }
            else
            {
                sessionScreen.UpdateInputMessage("That is not a number, please try again!");
            }
        }

        public void HandleEditorScreenCommands()
        {
            // de huidige vraag tonen
            Questions questions = new Questions();
            EditorScreen editorScreen = _screenHandler.Screen as EditorScreen;

            List<string> answers = new();

            editorScreen.DrawHeader(questions.WELCOME);
            
            int i = 0;
            while (i < questions.EditorQuestions.Count)
            {
                editorScreen.UpdateLastQuestion(questions.EditorQuestions.ElementAt(i));

                var input = Console.ReadLine();
                Console.WriteLine(input);

                if (input.Equals("break"))
                {
                    break;
                }

                if (questions.EditorAnswers.ElementAt(i).Contains(input))
                {
                    answers.Add(input);
                    i++;
                    Console.Clear();
                }
                else
                {
                    editorScreen.PrintWarning("Please fill in an valid answer");
                } 
            }

            if (answers.ElementAt(2).Contains("yes"))
            {
                Console.WriteLine("BINNEN CUSTOM COMBAT RULE");
                //customCombatRuleHandleEditorScreenCommands();
            }

            if (answers.ElementAt(3).Contains("yes"))
            {
                Console.WriteLine("BINNEN CUSTOM EXPLORE RULE");
                //customExploreRuleHandleEditorScreenCommands();
            }

            //naar de volgende scherm gaan!
        }

        private string customCombatRuleHandleEditorScreenCommands()
        {
            StringBuilder builder = new StringBuilder();
            BaseVariables variables = new();
            bool nextLine = true;
            EditorScreen editorScreen = _screenHandler.Screen as EditorScreen;

            editorScreen.UpdateLastQuestion("this is and example line: When player nearby player then attack" + Environment.NewLine + "press enter to continue...");
            
            Console.ReadLine();
            Console.Clear();

            while (nextLine)
            {
                editorScreen.UpdateLastQuestion("when ....." + Environment.NewLine + variables.comparebles.ToString()) ;
                var input = Console.ReadLine();

                if (variables.consumables.Contains(input)){

                    if (input.Equals("item"))
                    {
                        //zoeken welk item?
                    }
                    else
                    {
                        Console.Clear();
                        editorScreen.UpdateLastQuestion(input + "........" + Environment.NewLine + variables.comparison.ToString());
                    }
                }
            }

            return builder.ToString();
        }

        private string customExploreRuleHandleEditorScreenCommands()
        {
            StringBuilder builder = new StringBuilder();


            return builder.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Agent.Services;
using ASD_Game.InputHandling.Antlr;
using ASD_Game.InputHandling.Models;
using ASD_Game.Messages;
using ASD_Game.Session;
using ASD_Game.Session.GameConfiguration;
using ASD_Game.UserInterface;
using InputCommandHandler.Models;
using Session;
using WebSocketSharp;

namespace ASD_Game.InputHandling
{
    public class InputHandler : IInputHandler
    {
        private readonly IPipeline _pipeline;
        private readonly ISessionHandler _sessionHandler;
        private readonly IScreenHandler _screenHandler;
        private static Timer _timer;
        private readonly IMessageService _messageService;
        private readonly IGameConfigurationHandler _gameConfigurationHandler;
        private readonly IGamesSessionService _gamesSessionService;

        private const string RETURN_KEYWORD = "return";
        private const string START_COMMAND = "start_session";

        public InputHandler(IPipeline pipeline, ISessionHandler sessionHandler, IScreenHandler screenHandler, IMessageService messageService, IGameConfigurationHandler gameConfigurationHandler, IGamesSessionService gamesSessionService)
        {
            _pipeline = pipeline;
            _sessionHandler = sessionHandler;
            _screenHandler = screenHandler;
            _gameConfigurationHandler = gameConfigurationHandler;
            _gamesSessionService = gamesSessionService;
            _messageService = messageService;
        }

        public InputHandler(IGamesSessionService gamesSessionService)
        {
            _gamesSessionService = gamesSessionService;
            //Empty constructor needed for testing purposes
        }

        public void HandleGameScreenCommands()
        {
            SendCommand(GetCommand());
            _screenHandler.RedrawGameInputBox();
        }

        // Needed to handle input on client when host has started session
        public void HandleGameScreenCommands(string input)
        {
            SendCommand(input);
            _screenHandler.RedrawGameInputBox();
        }

        private void SendCommand(string commando)
        {
            try
            {
                _pipeline.ParseCommand(commando);
                _pipeline.Transform();
            }
            catch (Exception e)
            {
                _messageService.AddMessage(e.Message);
            }
        }

        public virtual string GetCommand()
        {
            return _screenHandler.GetScreenInput();
        }

        public void HandleStartScreenCommands()
        {
            var input = GetCommand();
            int.TryParse(input, out var option);

            switch (option)
            {
                case 1:
                    _screenHandler.TransitionTo(new ConfigurationScreen());
                    break;
                case 2:
                    _screenHandler.TransitionTo(new SessionScreen());
                    _sessionHandler.RequestSessions();
                    break;
                case 3:
                    _screenHandler.TransitionTo(new LoadScreen());
                    _gamesSessionService.RequestSavedGames();
                    break;
                case 4:
                    _screenHandler.TransitionTo(new EditorScreen());
                    break;
                case 5:
                    SendCommand("exit");
                    break;
                default:
                    var startScreen = _screenHandler.Screen as StartScreen;
                    startScreen.UpdateInputMessage("Not a valid option, try again!");
                    break;
            }
        }

        public void HandleSessionScreenCommands()
        {
            SessionScreen sessionScreen = _screenHandler.Screen as SessionScreen;
            var input = GetCommand();
            // Needed to handle input on client when host has started session
            if (_screenHandler.Screen is GameScreen)
            {
                HandleGameScreenCommands(input);
            }
            else
            {
                if (input == RETURN_KEYWORD)
                {
                    _screenHandler.TransitionTo(new StartScreen());
                    return;
                }

                string[] inputParts = input.Split(" ");

                if (inputParts.Length != 2)
                {
                    sessionScreen.UpdateInputMessage("Provide both a session number and username (example: 1 Gerrit)");
                }
                else
                {
                    int sessionNumber = 0;
                    int.TryParse(input[0].ToString(), out sessionNumber);

                    string sessionId = sessionScreen.GetSessionIdByVisualNumber(sessionNumber - 1);

                    if (sessionId.IsNullOrEmpty())
                    {
                        sessionScreen.UpdateInputMessage("Not a valid session, try again!");
                    }
                    else
                    {
                        _sessionHandler.JoinSession(sessionId, inputParts[1].Replace("\"", ""));
                    }
                }
            }
        }

        public void HandleLobbyScreenCommands()
        {
            var input = GetCommand();
            // Needed to handle input on client when host has started session
            if (_screenHandler.Screen is GameScreen)
            {
                HandleGameScreenCommands(input);
            }
            else
            {
                if (input == RETURN_KEYWORD)
                {
                    _screenHandler.TransitionTo(new StartScreen());
                    return;
                }

                if (input == START_COMMAND) 
                {
                    SendCommand(START_COMMAND);
                }

                if (input.Contains("say"))
                {
                    SendCommand(input);
                }
                else if (input.Contains("shout"))
                {
                    SendCommand(input);
                }
               
            }
        }

        public void HandleLoadScreenCommands()
        {
            LoadScreen loadScreen = _screenHandler.Screen as LoadScreen;
            var input = GetCommand();

            if (input == RETURN_KEYWORD)
            {
                _screenHandler.TransitionTo(new StartScreen());
                return;
            }

            if (input.Length > 0)
            {
                int sessionNumber = 0;
                int.TryParse(input, out sessionNumber);

                string sessionId = _screenHandler.GetSessionByPosition(sessionNumber - 1);

                if (sessionId.IsNullOrEmpty())
                {
                    _screenHandler.UpdateInputMessage("Not a valid session number, please try again!");
                }
                else
                {
                    _screenHandler.TransitionTo(new LobbyScreen());
                    _gamesSessionService.LoadGame(sessionId);
                }
            }
            else
            {
                _screenHandler.UpdateInputMessage("Session number cannot be left blank, please try again!");
            }
        }

        public void HandleConfigurationScreenCommands()
        {
            var input = GetCommand();
            if (input == RETURN_KEYWORD)
            {
                _screenHandler.TransitionTo(new StartScreen());
                _gameConfigurationHandler.SetGameConfiguration();
            }
            else
            {
                _gameConfigurationHandler.SetCurrentScreen();

                var configurationCompleted = _gameConfigurationHandler.HandleAnswer(input);

                if (configurationCompleted)
                {
                    _gameConfigurationHandler.SetGameConfiguration();
                    _screenHandler.TransitionTo(new LobbyScreen());
                    _sessionHandler.CreateSession(_gameConfigurationHandler.GetSessionName(), _gameConfigurationHandler.GetUsername(), false, null, null);

                }
            }
        }

        public void HandleEditorScreenCommands()
        {
            var questions = new Questions();
            var editorScreen = _screenHandler.Screen as EditorScreen;

            List<string> answers = new() {"explore=", "combat=", "", ""};

            var i = 0;
            Thread.Sleep(100);
            while (i < questions.EditorQuestions.Count)
            {
                editorScreen.UpdateLastQuestion(questions.EditorQuestions.ElementAt(i));

                var input = _screenHandler.GetScreenInput();
                _screenHandler.SetScreenInput(input);

                if (input.Equals("break"))
                {
                    break;
                }

                if(input.Equals("help combat"))
                {
                    _screenHandler.ConsoleHelper.ClearConsole();
                    editorScreen.UpdateLastQuestion(questions.HelpCombat);
                    _screenHandler.ConsoleHelper.ReadLine();
                } else if(input.Equals("help explore"))
                {
                    _screenHandler.ConsoleHelper.ClearConsole();
                    editorScreen.UpdateLastQuestion(questions.HelpExplore);
                    _screenHandler.ConsoleHelper.ReadLine();
                } else if (questions.EditorAnswers.ElementAt(i).Contains(input))
                {
                    answers[i] = answers[i] + input;
                    i++;
                    _screenHandler.ConsoleHelper.ClearConsole();
                }
                else
                {
                    editorScreen.PrintWarning("Please fill in a valid answer");
                }
            }

            if (answers.Count == questions.EditorQuestions.Count)
            {
                if (answers.ElementAt(2).Contains("yes"))
                {
                    answers.Add(CustomRuleHandleEditorScreenCommands("combat"));
                }

                if (answers.ElementAt(3).Contains("yes"))
                {
                    answers.Add(CustomRuleHandleEditorScreenCommands("explore"));
                }
            }

            var finalString = "";
            foreach (var element in answers)
            {
                if (element != "yes" && element != "no")
                {
                    finalString += element + Environment.NewLine;
                }
            }

            var agentConfigurationService = new ConfigurationService();
            List<string> errors = agentConfigurationService.Configure(finalString);

/*            AgentService agentService = new AgentService();
            List<string> errors = agentService.Configure(finalString);*/
            string errorsCombined = string.Empty;

            if (errors.Count != 0)
            {
                foreach (var error in errors)
                {
                    errorsCombined += error + ", ";
                }
                editorScreen.UpdateLastQuestion(errorsCombined +
                                                "Please fix the errors and retry press enter to continue...");
                _screenHandler.GetScreenInput();
                editorScreen.ClearScreen();
                _screenHandler.TransitionTo(new StartScreen());
            }
            
            editorScreen.UpdateLastQuestion("Your agent has been configured successfully!" +
                                            "press enter to continue to the startscreen");
            _screenHandler.GetScreenInput();
            editorScreen.ClearScreen();
            _screenHandler.TransitionTo(new StartScreen());
        }

        public string CustomRuleHandleEditorScreenCommands(string type)
        {
            string startText = "Please enter your own " + type + " rule "
                               + " This is an example line: When agent nearby player then attack (optional: otherwise flee)";
            StringBuilder builder = new StringBuilder();
            BaseVariables variables = new();
            var nextLine = true;
            var editorScreen = _screenHandler.Screen as EditorScreen;

            string input;

            builder.Append(type + Environment.NewLine);

            editorScreen.UpdateLastQuestion("This is and example line: When agent nearby player then attack press enter to continue...");

            _screenHandler.GetScreenInput();
            editorScreen.ClearScreen();

            while (nextLine)
            {
                editorScreen.UpdateLastQuestion(
                    startText
                    + " Type Help + armor, weapon, comparison, consumables, actions, bitcoinItems, comparables for possibilities"
                    + " Type Stop to stop the custom rules editor");

                input = _screenHandler.GetScreenInput();
                input = input.ToLower();

                if (input.Equals("stop"))
                {
                    return string.Empty;
                }

                if (input.Contains("help"))
                {
                    var help = input.Split(" ");
                    switch (help[1])
                    {
                        case "armor":
                            editorScreen.ClearScreen();
                            editorScreen.UpdateLastQuestion("Possible armors: " +
                                                            string.Join(", ", variables.armor) +
                                                            startText);
                            break;
                        case "weapon":
                            editorScreen.ClearScreen();
                            editorScreen.UpdateLastQuestion("Possible weapons: " + 
                                                            string.Join(", ", variables.weapons) +
                                                            startText);
                            break;
                        case "comparison":
                            editorScreen.ClearScreen();
                            editorScreen.UpdateLastQuestion("Possible comparison: " + 
                                                            string.Join(", ",
                                                                variables.comparison) +
                                                            startText);
                            break;
                        case "consumables":
                            editorScreen.ClearScreen();
                            editorScreen.UpdateLastQuestion("Possible consumables: " +
                                                            string.Join(", ", variables.consumables) +
                                                            startText);
                            break;
                        case "actions":
                            editorScreen.ClearScreen();
                            editorScreen.UpdateLastQuestion("Possible actions: " + 
                                                            string.Join(", ", variables.actions) +
                                                            startText);
                            break;
                        case "bitcoinitems":
                            editorScreen.ClearScreen();
                            editorScreen.UpdateLastQuestion("Possible bitcoin items: " + 
                                                            string.Join(", ", variables.bitcoinItems) +
                                                            startText);
                            break;
                        case "comparables":
                            editorScreen.ClearScreen();
                            editorScreen.UpdateLastQuestion("Possible comparables: " +
                                                            string.Join(", ", variables.comparables) +
                                                            startText);
                            break;
                    }

                    input = _screenHandler.GetScreenInput();
                    input = input.ToLower();
                }

                var rule = input.Split(" ").ToList();

                //Check if the user input match basic requirements
                if (CheckInput(rule, variables))
                {
                    builder.Append(input);
                }
                else
                {
                    editorScreen.ClearScreen();
                    editorScreen.UpdateLastQuestion("Your rule did not pass the check and was not added press enter to continue...");
                    _screenHandler.GetScreenInput();
                }

                while (true)
                {
                    editorScreen.ClearScreen();
                    editorScreen.UpdateLastQuestion("Do you want to add another rule? yes or no");
                    input = _screenHandler.GetScreenInput();
                    input = input.ToLower();
                    if (input.Equals("yes") || input.Equals("no"))
                    {
                        editorScreen.ClearScreen();
                        break;
                    }
                }

                if (input.Equals("no"))
                {
                    nextLine = false;
                }
            }

            return builder.ToString();
        }

        public bool CheckInput(List<string> rule, BaseVariables variables)
        {
            var correct = false;
            //basic rules
            //contains all two base words
            List<string> baseWords = new() {"when", "then"};
            correct = (rule.Intersect(baseWords).Count() == 2);
            if (!correct)
            {
                return correct;
            }

            //check positions of the base words
            correct = rule.IndexOf(baseWords[0]) == 0 && rule.IndexOf(baseWords[1]) == 4;
            if (!correct)
            {
                return correct;
            }

            //contains exactly 1 comparison type
            correct = rule.Intersect(variables.comparison).Any();
            if (!correct)
            {
                return correct;
            }

            correct = rule.IndexOf(variables.comparison.FirstOrDefault(x => x.Equals(rule[2]))) == 2;
            if (!correct)
            {
                return correct;
            }

            //check otherwise
            correct = rule.IndexOf("otherwise") == 6 || rule.IndexOf("otherwise") == 7 || rule.IndexOf("otherwise") < 0;
            if (!correct)
            {
                return correct;
            }

            //advanced rules
            correct = (rule.Intersect(variables.actionReferences).Any() || rule.Intersect(variables.actions).Any());
            if (correct && rule.Contains("use"))
            {
                correct = rule.Intersect(variables.ReturnAllItems()).Any();
            }

            if (!correct)
            {
                return correct;
            }

            //check first variable is of type comparebles
            correct = variables.comparables.Contains(rule[1]);
            if (!correct)
            {
                return correct;
            }

            //check second variable is of type item or interger
            correct = rule.IndexOf(variables.ReturnAllItems().FirstOrDefault(x => x.Equals(rule[3]))) == 3 ||
                      int.TryParse(rule[3], out _) ||
                      rule.FindLastIndex(x => x.Equals(variables.comparables.FirstOrDefault(x => x.Equals(rule[3])))) ==
                      3;
            if (!correct)
            {
                return correct;
            }

            //check is use count matches item count
            correct = rule.Count(x => x.Equals("use")) == variables.ReturnAllItems().Intersect(rule).Count();
            if (!correct)
            {
                return correct;
            }

            return correct;
        }
    }
}
using System;
using System.Timers;
using ASD_Game.InputHandling.Antlr;
using ASD_Game.Messages;
using ASD_Game.Session;
using ASD_Game.Session.GameConfiguration;
using ASD_Game.UserInterface;
using Session;
using WebSocketSharp;

namespace ASD_Game.InputHandling
{
    public class InputHandler : IInputHandler
    {
        private readonly IPipeline _pipeline;
        private readonly ISessionHandler _sessionHandler;
        private readonly IScreenHandler _screenHandler;
        private readonly IGameConfigurationHandler _gameConfigurationHandler;
        private readonly IMessageService _messageService;
        private static Timer aTimer;
        private const string RETURN_KEYWORD = "return";
        private string _enteredSessionName;
        private readonly IGamesSessionService _gamesSessionService;

        public string START_COMMAND = "start_session";

        public InputHandler(IPipeline pipeline, ISessionHandler sessionHandler, IScreenHandler screenHandler,
            IMessageService messageService, IGameConfigurationHandler gameConfigurationHandler,
            IGamesSessionService gamesSessionService)
        {
            _pipeline = pipeline;
            _sessionHandler = sessionHandler;
            _screenHandler = screenHandler;
            _gameConfigurationHandler = gameConfigurationHandler;
            _gamesSessionService = gamesSessionService;
            _messageService = messageService;
        }

        public InputHandler()
        {
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
                    _gamesSessionService.RequestSavedGames();
                    break;
                case 4:
                    _screenHandler.TransitionTo(new EditorScreen());
                    break;
                case 5:
                    SendCommand("exit");
                    break;
                default:
                    StartScreen startScreen = _screenHandler.Screen as StartScreen;
                    startScreen.UpdateInputMessage("Not a valid option, try again!");
                    break;
            }
        }

        public void HandleSessionScreenCommands()
        {
            SessionScreen sessionScreen = _screenHandler.Screen as SessionScreen;
            var input = GetCommand();

            if (input == RETURN_KEYWORD)
            {
                _screenHandler.TransitionTo(new StartScreen());
                return;
            }

            var inputParts = input.Split(" ");

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
                else if (_sessionHandler.NotAllowedToJoin)
                {
                    _screenHandler.UpdateInputMessage("Not allowed to join running game");
                }
                else
                {
                    _screenHandler.TransitionTo(new LobbyScreen());
                    SendCommand("join_session \"" + sessionId + "\" \"" + inputParts[1].Replace("\"", "") + "\"");
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

                if (input.Contains("SAY"))
                {
                    SendCommand(input);
                }
                else if (input.Contains("SHOUT"))
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
                bool configurationCompleted = _gameConfigurationHandler.HandleAnswer(input);

                if (configurationCompleted)
                {
                    _gameConfigurationHandler.SetGameConfiguration();
                    _sessionHandler.CreateSession(_gameConfigurationHandler.GetSessionName(),
                        _gameConfigurationHandler.GetUsername(), false, null, null);
                    _screenHandler.TransitionTo(new LobbyScreen());
                }
            }
        }
    }
}
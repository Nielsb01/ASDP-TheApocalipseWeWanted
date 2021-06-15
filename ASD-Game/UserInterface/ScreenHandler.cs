using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ASD_Game.UserInterface
{
    public class ScreenHandler : IScreenHandler
    {
        private Screen _screen = null;
        public Screen Screen { get => _screen; set => _screen = value; }
        private ConsoleHelper _consoleHelper;
        public ConsoleHelper ConsoleHelper { get => _consoleHelper; set => _consoleHelper = value; }
        private BlockingCollection<Action> _actionsInQueue;
        public BlockingCollection<Action> ActionsInQueue
        {
            get => _actionsInQueue;
            set => _actionsInQueue = value;
        }
        private Thread _displayThread { get; set; }
        
        private bool _displaying;
        
        public ScreenHandler()
        {
            _consoleHelper = new ConsoleHelper();
            ActionsInQueue = new();

            _displayThread = new Thread(RunDisplay);
            _displayThread.Start();
        }

        private void RunDisplay()
        {
            while(ActionsInQueue.TryTake(out Action a, -1))
            {
                a.Invoke();
            }
        }

        public virtual void TransitionTo(Screen screen)
        {
            _consoleHelper.ClearConsole();
            _screen = screen;
            _screen.SetScreen(this);
            DisplayScreen();
        }
        public void DisplayScreen()
        {
            ActionsInQueue.Add(_screen.DrawScreen);
        }

        public void ShowMessages(Queue<string> messages)
        {
            if (_screen is GameScreen)
            {
                var gameScreen = Screen as GameScreen;
                ActionsInQueue.Add(() => gameScreen.ShowMessages(messages));
                _actionsInQueue.Add(() => gameScreen.ShowMessages(messages));
            } else if (_screen is LobbyScreen)
            {
                var lobbyScreen = Screen as LobbyScreen;
                _actionsInQueue.Add(() => lobbyScreen.ShowMessages(messages));
            }
        }

        public virtual string GetScreenInput()
        {
            return _consoleHelper.ReadLine();
        }

        public void RedrawGameInputBox()
        {
            if (_screen is GameScreen)
            {
                var gameScreen = Screen as GameScreen;
                ActionsInQueue.Add(gameScreen.RedrawInputBox);
                _displayThread = new Thread(gameScreen.RedrawInputBox);
                _actionsInQueue.Add(gameScreen.RedrawInputBox);
            } 
            else if (_screen is LobbyScreen)
            {
                var lobbyScreen = Screen as LobbyScreen;
                _actionsInQueue.Add(lobbyScreen.RedrawInputBox);
            }
        }

        public void UpdateWorld(char[,] map)
        {
            if (_screen is GameScreen)
            {
                var gameScreen = Screen as GameScreen;
                ActionsInQueue.Add(() => gameScreen.UpdateWorld(map));
            }
        }

        public void SetStatValues(string name, int score, int playersAlive, int playersTotal, int health, int stamina, int armor, int radiation, string helm, string body, string weapon, string slotOne, string slotTwo, string slotThree)
        {
            if (_screen is GameScreen)
            {
                GameScreen gameScreen = _screen as GameScreen;
                ActionsInQueue.Add(() => gameScreen.SetStatValues(name, score, health, stamina, armor, radiation, helm, body, weapon, slotOne, slotTwo, slotThree));
            }
        }

        public void UpdateSavedSessionsList(List<string[]> sessions)
        {
            if (_screen is LoadScreen)
            {
                LoadScreen loadScreen = _screen as LoadScreen;
                loadScreen.UpdateSavedSessionsList(sessions);
            }
        }

        public virtual void SetScreenInput(string input)
        {
            _consoleHelper.WriteLine(input);
        }

        public virtual void UpdateInputMessage(string message)
        {
            if (_screen is LoadScreen screen)
            {
                screen.UpdateInputMessage(message);
            }
        }

        public virtual string GetSessionByPosition(int sessionNumber)
        {
            if (_screen is LoadScreen screen)
            {
                return screen.GetSessionByPosition(sessionNumber);
            }
            
            return String.Empty;
        }
    }
}
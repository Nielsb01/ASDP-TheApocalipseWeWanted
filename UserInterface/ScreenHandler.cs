using System;
using System.Collections.Generic;

namespace UserInterface
{
    public class ScreenHandler : IScreenHandler
    {
        private Screen _screen = null;
        public Screen Screen { get => _screen; set => _screen = value; }
        private ConsoleHelper _consoleHelper;
        public ConsoleHelper ConsoleHelper { get => _consoleHelper; set => _consoleHelper = value; }
        public ScreenHandler()
        {
            _consoleHelper = new ConsoleHelper();
        }
        public void TransitionTo(Screen screen)
        {
            _consoleHelper.ClearConsole();
            _screen = screen;
            _screen.SetScreen(this);
            DisplayScreen();
        }
        public void DisplayScreen()
        {
            _screen.DrawScreen();
        }

        public void ShowMessages(Queue<string> messages)
        {
            if(_screen is GameScreen)
            {
                var gameScreen = Screen as GameScreen;
                gameScreen.ShowMessages(messages);
            }
        }

        public string GetScreenInput()
        {
            return _consoleHelper.ReadLine();
        }
    }
}
using System.Collections.Generic;
using System.Threading;

namespace UserInterface
{
    public class ScreenHandler : IScreenHandler
    {
        private Screen _screen = null;
        public Screen Screen { get => _screen; set => _screen = value; }
        private ConsoleHelper _consoleHelper;
        public ConsoleHelper ConsoleHelper { get => _consoleHelper; set => _consoleHelper = value; }
        public static Thread DisplayThread { get; set; }
        
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
            if(DisplayThread != null)
            {
                DisplayThread.Join();
            }
            DisplayThread = new Thread(new ThreadStart(_screen.DrawScreen));
        }

        public void ShowMessages(Queue<string> messages)
        {
            if (_screen is GameScreen)
            {
                var gameScreen = Screen as GameScreen;
                DisplayThread.Join();
                DisplayThread = new Thread(new ParameterizedThreadStart(empty => gameScreen.ShowMessages(messages)));
            }
        }

        public string GetScreenInput()
        {
            return _consoleHelper.ReadLine();
        }

        public void RedrawGameInputBox()
        {
            if (_screen is GameScreen)
            {
                var gameScreen = Screen as GameScreen;
                DisplayThread.Join();
                DisplayThread = new Thread(new ThreadStart(gameScreen.RedrawInputBox));
            }
        }

        public void UpdateWorld(char[,] map)
        {
            if (_screen is GameScreen)
            {
                var gameScreen = Screen as GameScreen;
                DisplayThread.Join();
                DisplayThread = new Thread(new ParameterizedThreadStart(empty => gameScreen.UpdateWorld(map)));
            }
        }

        public void SetStatValues(string name, int score, int health, int stamina, int armor, int radiation, string helm, string body, string weapon, string slotOne, string slotTwo, string slotThree)
        {
            if (_screen is GameScreen)
            {
                GameScreen gameScreen = _screen as GameScreen;
                DisplayThread.Join();
                DisplayThread = new Thread(new ParameterizedThreadStart(empty => gameScreen.SetStatValues(name, score, health, stamina, armor, radiation, helm, body, weapon, slotOne, slotTwo, slotThree)));
            }
        }
    }
}
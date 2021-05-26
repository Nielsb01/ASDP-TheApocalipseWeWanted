using System;

namespace UserInterface
{
    public class ScreenHandler : IScreenHandler
    {
        private Screen _screen = null;
        
        public void TransitionTo(Screen screen)
        {
            Console.Clear();
            _screen = screen;
            _screen.SetScreen(this);
            DisplayScreen();
        }
        public void DisplayScreen()
        {
            _screen.DrawScreen();
        }

        public void AcceptInput()
        {
            _screen.HandleInput();
        }
    }
}
namespace ASD_project.InputHandling
{
    public interface IInputHandler
    {
        public void HandleGameScreenCommands();
        public void HandleStartScreenCommands();
        public void HandleSessionScreenCommands();
    }
}
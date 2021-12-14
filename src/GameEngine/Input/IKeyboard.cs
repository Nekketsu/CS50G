namespace GameEngine.Input
{
    public interface IKeyboard
    {
        event EventHandler<Key> KeyPressed;

        Task Initialize();
        bool IsDown(Key key);
        Task DisposeAsync();
    }
}

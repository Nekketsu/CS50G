namespace GameEngine.Input
{
    public class StatefulKeyboard : IKeyboard
    {
        private readonly IKeyboard keyboard;

        HashSet<Key> keysPressed { get; }

        public StatefulKeyboard(IKeyboard keyboard)
        {
            // initialize input table
            keysPressed = new HashSet<Key>();

            this.keyboard = keyboard;

            keyboard.KeyPressed += OnKeyPressed; ;
        }

        public event EventHandler<Key> KeyPressed;

        private void OnKeyPressed(object sender, Key key)
        {
            // add to our table of key pressed this frame
            keysPressed.Add(key);

            KeyPressed?.Invoke(sender, key);
        }

        public Task Initialize()
        {
            return keyboard.Initialize();
        }

        public bool IsDown(Key key)
        {
            return keyboard.IsDown(key);
        }

        // Custom function to extend LÖVE's input handling; returns whether a given
        // key was set to true in our input table this frame.
        public bool WasPressed(Key key)
        {
            return keysPressed.Contains(key);
        }

        public void Update()
        {
            keysPressed.Clear();
        }

        public async Task DisposeAsync()
        {
            keyboard.KeyPressed -= OnKeyPressed;

            await keyboard.DisposeAsync();
        }
    }
}

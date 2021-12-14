namespace GameEngine.Input
{
    public class StatefulMouse : IMouse
    {
        private readonly IMouse mouse;

        public HashSet<MouseButton> ButtonsPressed { get; private set; }

        public StatefulMouse(IMouse mouse)
        {
            // initialize mouse input table
            ButtonsPressed = new HashSet<MouseButton>();

            this.mouse = mouse;

            mouse.MousePressed += OnMousePressed;
        }

        private void OnMousePressed(object sender, MouseButtonArgs button)
        {
            ButtonsPressed.Add(button.Button);

            MousePressed?.Invoke(sender, button);
        }

        // Equivalent to our keyboard function from before, but for the mouse buttons.
        public bool WasPressed(MouseButton button)
        {
            return ButtonsPressed.Contains(button);
        }

        public event EventHandler<MouseButtonArgs> MousePressed;

        public void Update()
        {
            ButtonsPressed.Clear();
        }

        public Task Initialize()
        {
            return mouse.Initialize();
        }

        public async Task DisposeAsync()
        {
            mouse.MousePressed -= OnMousePressed;

            await mouse.DisposeAsync();
        }
    }
}

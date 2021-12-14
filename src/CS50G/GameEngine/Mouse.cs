using GameEngine.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CS50G.GameEngine
{
    public class Mouse : IMouse
    {
        private IJSRuntime jsRuntime;
        private ElementReference canvas;

        Dictionary<MouseButton, bool> mouseButtonsPressed;

        public Mouse(IJSRuntime jsRuntime, ElementReference canvas)
        {
            this.jsRuntime = jsRuntime;
            this.canvas = canvas;

            mouseButtonsPressed = new Dictionary<MouseButton, bool>();
            foreach (var mouseButton in Enum.GetValues(typeof(MouseButton)).Cast<MouseButton>())
            {
                mouseButtonsPressed.Add(mouseButton, false);
            }
        }

        public async Task Initialize()
        {
            await jsRuntime.InvokeVoidAsync("mouse.initialize", DotNetObjectReference.Create(this), canvas);
        }

        public event EventHandler<MouseButtonArgs> MousePressed;

        [JSInvokable]
        public async Task OnMouseDown(int x, int y, int button)
        {
            if (Enum.IsDefined(typeof(MouseButton), button))
            {
                var mouseButton = (MouseButton)button;
                mouseButtonsPressed[mouseButton] = true;

                MousePressed(this, new MouseButtonArgs(x, y, mouseButton));
            }

            await Task.CompletedTask;
        }

        [JSInvokable]
        public async Task OnMouseUp(int x, int y, int button)
        {
            if (Enum.IsDefined(typeof(MouseButton), button))
            {
                var mouseButton = (MouseButton)button;
                mouseButtonsPressed[mouseButton] = true;
            }

            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await jsRuntime.InvokeVoidAsync("keyboard.dispose");
        }
    }
}

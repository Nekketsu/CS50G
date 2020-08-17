using GameEngine.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS50G.GameEngine
{
    public class Keyboard : IKeyboard
    {
        private readonly IJSRuntime jsRuntime;
        private readonly ElementReference canvas;

        public event EventHandler<Key> KeyPressed;

        Dictionary<Key, bool> keysPressed;
        Dictionary<string, Key> keyMapping;

        public Keyboard(IJSRuntime jsRuntime, ElementReference canvas)
        {
            this.jsRuntime = jsRuntime;
            this.canvas = canvas;

            keysPressed = new Dictionary<Key, bool>();
            foreach (var key in Enum.GetValues(typeof(Key)).Cast<Key>())
            {
                keysPressed.Add(key, false);
            }

            keyMapping = new Dictionary<string, Key>
            {
                ["Enter"] = Key.Enter,
                ["NumpadEnter"] = Key.Return,
                ["Space"] = Key.Space,
                ["Escape"] = Key.Escape,
                ["ArrowLeft"] = Key.Left,
                ["ArrowUp"] = Key.Up,
                ["ArrowRight"] = Key.Right,
                ["ArrowDown"] = Key.Down,
                ["KeyW"] = Key.W,
                ["KeyS"] = Key.S
            };
        }

        public async Task Initialize()
        {
            await jsRuntime.InvokeVoidAsync("keyboard.initialize", DotNetObjectReference.Create(this), canvas);
        }

        public bool IsDown(Key key)
        {
            return keysPressed[key];
        }

        [JSInvokable]
        public async Task OnKeyDown(string code)
        {
            if (keyMapping.TryGetValue(code, out var key))
            {
                keysPressed[key] = true;
                KeyPressed(this, key);
            }

            await Task.CompletedTask;
        }

        [JSInvokable]
        public async Task OnKeyUp(string code)
        {
            if (keyMapping.TryGetValue(code, out var key))
            {
                keysPressed[key] = false;
            }

            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await jsRuntime.InvokeVoidAsync("keyboard.dispose");
        }
    }
}

using GameEngine.Audio;
using GameEngine.Graphics;
using GameEngine.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameEngine
{
    public abstract class Game
    {
        public static Game Instance { get; private set; }

        public string GameName { get; }

        private readonly IGameLoop gameLoop;

        public IGraphics Graphics { get; }
        public IAudio Audio { get; }
        public IKeyboard Keyboard { get; }
        public IMouse Mouse { get; }

        public Dictionary<string, ISource> Sounds { get; }

        protected Game(string gameName, IGameLoop gameLoop, IGraphics graphics, IAudio audio, IKeyboard keyboard, IMouse mouse = null)
        {
            GameName = gameName;
            this.gameLoop = gameLoop;
            Graphics = graphics;
            Audio = audio;
            Keyboard = keyboard;
            Mouse = mouse;
            Sounds = new Dictionary<string, ISource>();

            Instance = this;
        }

        public abstract int VirtualWidth { get; }
        public abstract int VirtualHeight { get; }

        public virtual async Task Load() { await Task.CompletedTask; }
        public virtual async Task Update(TimeSpan dt) { await Task.CompletedTask; }
        public virtual void KeyPressed(Key key) { }
        public virtual void MousePressed(int x, int y, MouseButton button) { }
        public virtual async Task Draw() { await Task.CompletedTask; }

        public async Task Initialize()
        {
            await gameLoop.Initialize();
            await Graphics.Initialize();
            await Keyboard.Initialize();
            if (Mouse != null) { await Mouse?.Initialize(); }

            gameLoop.OnUpdate += OnUpdate;
            Keyboard.KeyPressed += OnKeyPressed;
            if (Mouse != null)
            {
                Mouse.MousePressed += OnMousePressed;
            }
        }

        public async Task Run()
        {
            await Initialize();
            await Load();

            await gameLoop.Run();
        }

        private async void OnUpdate(object sender, TimeSpan time)
        {
            await Update(time);
            await Draw();
        }

        private void OnKeyPressed(object sender, Key key)
        {
            KeyPressed(key);
        }

        private void OnMousePressed(object sender, MouseButtonArgs button)
        {
            MousePressed(button.X, button.Y, button.Button);
        }

        public async Task DisposeAsync()
        {
            await gameLoop.Stop();
            await Keyboard.DisposeAsync();
            if (Mouse != null) { await Mouse.DisposeAsync(); }

            gameLoop.OnUpdate -= OnUpdate;
            Keyboard.KeyPressed -= OnKeyPressed;
            if (Mouse != null) { Mouse.MousePressed -= OnMousePressed; }

            var loopingSounds = Sounds.Values.Where(sound => sound.Looping);
            foreach (var sound in loopingSounds)
            {
                await sound.Stop();
            }

            Instance = null;
        }
    }
}

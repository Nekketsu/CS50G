using GameEngine.Audio;
using GameEngine.Graphics;
using GameEngine.Input;
using System;
using System.Threading.Tasks;

namespace GameEngine
{
    public abstract class Game
    {
        public static Game Instance { get; private set; }

        private readonly IGameLoop gameLoop;

        public string GameName { get; }
        public IGraphics Graphics { get; }
        public IAudio Audio { get; }
        public IKeyboard Keyboard { get; }

        protected Game(string gameName, IGameLoop gameLoop, IGraphics graphics, IAudio audio, IKeyboard keyboard)
        {
            GameName = gameName;
            this.gameLoop = gameLoop;
            Graphics = graphics;
            Audio = audio;
            Keyboard = keyboard;

            Instance = this;
        }

        public abstract int VirtualWidth { get; }
        public abstract int VirtualHeight { get; }

        public virtual async Task Load() { await Task.CompletedTask; }
        public virtual async Task Update(TimeSpan dt) { await Task.CompletedTask; }
        public virtual void KeyPressed(Key key) { }
        public virtual async Task Draw() { await Task.CompletedTask; }

        public async Task Initialize()
        {
            await gameLoop.Initialize();
            await Graphics.Initialize();
            await Keyboard.Initialize();

            gameLoop.OnUpdate += OnUpdate;
            Keyboard.KeyPressed += (sender, key) => KeyPressed(key);
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
    }
}

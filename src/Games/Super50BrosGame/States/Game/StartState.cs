using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;

namespace Super50BrosGame.States.Game
{
    // StartState Class
    public class StartState : State
    {
        public GameLevel Map { get; private set; }
        public int Background { get; private set; }

        public override Task Enter(Dictionary<string, object> parameters)
        {
            var levelMaker = new LevelMaker();

            Map = levelMaker.Generate(100, 10);
            Background = Super50Bros.Instance.Random.Next(3);

            var height = Super50Bros.Instance.Textures["backgrounds"].Height;

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            if (Super50Bros.Instance.Keyboard.WasPressed(Key.Enter) || Super50Bros.Instance.Keyboard.WasPressed(Key.Return))
            {
                await Super50Bros.Instance.StateMachine.Change("play");
            }
        }

        public override Task Render()
        {
            Super50Bros.Instance.Graphics.Draw(Super50Bros.Instance.Textures["backgrounds"], Super50Bros.Instance.Frames["backgrounds"][Background], 0, 0);
            Super50Bros.Instance.Graphics.Draw(Super50Bros.Instance.Textures["backgrounds"], Super50Bros.Instance.Frames["backgrounds"][Background], 0,
                Super50Bros.Instance.Textures["backgrounds"].Height * 2 / 3, 1, -1);
            Map.Render();

            Super50Bros.Instance.Graphics.SetFont(Super50Bros.Instance.Fonts["title"]);
            Super50Bros.Instance.Graphics.SetColor(0, 0, 0, 255);
            Super50Bros.Instance.Graphics.Print($"Super 50 Bros.", 1, Super50Bros.Instance.VirtualHeight / 2 - 40 + 1, Super50Bros.Instance.VirtualWidth, Alignment.Center);
            Super50Bros.Instance.Graphics.SetColor(255, 255, 255, 255);
            Super50Bros.Instance.Graphics.Print($"Super 50 Bros.", 0, Super50Bros.Instance.VirtualHeight / 2 - 40, Super50Bros.Instance.VirtualWidth, Alignment.Center);

            Super50Bros.Instance.Graphics.SetFont(Super50Bros.Instance.Fonts["medium"]);
            Super50Bros.Instance.Graphics.SetColor(0, 0, 0, 255);
            Super50Bros.Instance.Graphics.Print($"Press Enter", 1, Super50Bros.Instance.VirtualHeight / 2 + 17, Super50Bros.Instance.VirtualWidth, Alignment.Center);
            Super50Bros.Instance.Graphics.SetColor(255, 255, 255, 255);
            Super50Bros.Instance.Graphics.Print($"Press Enter", 0, Super50Bros.Instance.VirtualHeight / 2 + 16, Super50Bros.Instance.VirtualWidth, Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

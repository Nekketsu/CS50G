using GameEngine;
using GameEngine.Audio;
using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using GameEngine.Timers;
using Match3Game.States;

namespace Match3Game
{
    public class Match3 : Game
    {
        public static new Match3 Instance { get; private set; }

        public new StatefulKeyboard Keyboard { get; }

        public Match3(IGameLoop gameLoop, IGraphics graphics, IAudio audio, IKeyboard keyboard, IMouse mouse = null, IFileSystem fileSystem = null) : base(nameof(Match3), gameLoop, graphics, audio, keyboard, mouse, fileSystem)
        {
            Keyboard = new StatefulKeyboard(keyboard);

            Instance = this;
        }

        public override int VirtualWidth => 512;
        public override int VirtualHeight => 288;

        const int BackgroundScrollSpeed = 80;

        public Dictionary<string, Font> Fonts { get; private set; }
        public Dictionary<string, Image> Textures { get; private set; }
        public Dictionary<string, Quad[][]> Frames { get; private set; }
        public StateMachine StateMachine { get; private set; }

        public GameEngine.Timers.Timer Timer { get; private set; }

        double backgroundX;

        public override async Task Load()
        {
            Sounds.Add("music", await Audio.NewSource("sounds/music3.mp3"));
            Sounds.Add("select", await Audio.NewSource("sounds/select.wav"));
            Sounds.Add("error", await Audio.NewSource("sounds/error.wav"));
            Sounds.Add("match", await Audio.NewSource("sounds/match.wav"));
            Sounds.Add("clock", await Audio.NewSource("sounds/clock.wav"));
            Sounds.Add("game-over", await Audio.NewSource("sounds/game-over.wav"));
            Sounds.Add("next-level", await Audio.NewSource("sounds/next-level.wav"));

            Textures = new Dictionary<string, Image>
            {
                ["main"] = await Graphics.NewImage("graphics/match3.png"),
                ["background"] = await Graphics.NewImage("graphics/background.png"),
            };

            var util = new Util();
            Frames = new Dictionary<string, Quad[][]>
            {
                // divided into sets for each tile type in this game, instead of one large
                // table of Quads
                ["tiles"] = util.GenerateTileQuads(Textures["main"])
            };

            // this time, we're keeping our fonts in a global table for readability
            Fonts = new Dictionary<string, Font>
            {
                ["small"] = await Graphics.NewFont("fonts/font.ttf", 8),
                ["medium"] = await Graphics.NewFont("fonts/font.ttf", 16),
                ["large"] = await Graphics.NewFont("fonts/font.ttf", 32)
            };


            // set music to loop and start
            Sounds["music"].Looping = true;
            await Sounds["music"].Play();

            Timer = new GameEngine.Timers.Timer();

            // initialize state machine with all state-returning functions
            StateMachine = new StateMachine(new Dictionary<string, State>
            {
                ["start"] = new StartState(),
                ["begin-game"] = new BeginGameState(),
                ["play"] = new PlayState(),
                ["game-over"] = new GameOverState()
            });

            await StateMachine.Change("start");

            // keep track of scrolling our background on the X axis
            backgroundX = 0;
        }

        public override async Task Update(TimeSpan dt)
        {
            // scroll background, used across all states
            backgroundX -= BackgroundScrollSpeed * dt.TotalSeconds;

            // if we've scrolled the entire image, reset it to 0
            if (backgroundX <= -1024 + VirtualWidth - 4 + 51)
            {
                backgroundX = 0;
            }

            await StateMachine.Update(dt);
            await Timer.Update(dt);
            Keyboard.Update();
        }

        public override async Task Draw()
        {
            Graphics.Begin();

            // scrolling background drawn behind every state
            Graphics.Draw(Textures["background"], backgroundX, 0);

            await StateMachine.Render();

            await Graphics.Apply();
        }
    }
}

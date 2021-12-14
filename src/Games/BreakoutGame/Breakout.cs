using BreakoutGame.States;
using GameEngine;
using GameEngine.Audio;
using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using System.Text;

namespace BreakoutGame
{
    // Breakout Remake

    // Originally developed by Atari in 1976. An effective evolution of
    // Pong, Breakout ditched the two-player mechanic in favor of a single-
    // player game where the player, still controlling a paddle, was tasked
    // with eliminating a screen full of differently placed bricks of varying
    // values by deflecting a ball back at them.

    // This version is built to more closely resemble the NES than
    // the original Pong machines or the Atari 2600 in terms of
    // resolution, though in widescreen (16:9) so it looks nicer on 
    // modern systems.

    // Credit for graphics (amazing work!):
    // https://opengameart.org/users/buch

    // Credit for music (great loop):
    // http://freesound.org/people/joshuaempyre/sounds/251461/
    // http://www.soundcloud.com/empyreanma
    public class Breakout : Game
    {
        public static new Breakout Instance { get; private set; }

        public new StatefulKeyboard Keyboard { get; }

        public Breakout(IGameLoop gameLoop, IGraphics graphics, IAudio audio, IKeyboard keyboard, IFileSystem fileSystem) : base(nameof(Breakout), gameLoop, graphics, audio, keyboard, null, fileSystem)
        {
            Keyboard = new StatefulKeyboard(keyboard);

            // the state machine we'll be using to transition between various states
            // in our game instead of clumping them together in our update and draw
            // methods
            //
            // our current game state can be any of the following:
            // 1. 'start' (the beginning of the game, where we're told to press Enter)
            // 2. 'paddle-select' (where we get to choose the color of our paddle)
            // 3. 'serve' (waiting on a key press to serve the ball)
            // 4. 'play' (the ball is in play, bouncing between paddles)
            // 5. 'victory' (the current level is over, with a victory jingle)
            // 6. 'game-over' (the player has lost; display score and allow restart)
            StateMachine = new StateMachine(new Dictionary<string, State>
            {
                ["start"] = new StartState(),
                ["play"] = new PlayState(),
                ["serve"] = new ServeState(),
                ["game-over"] = new GameOverState(),
                ["victory"] = new VictoryState(),
                ["high-scores"] = new HighScoreState(),
                ["enter-high-score"] = new EnterHighScoreState(),
                ["paddle-select"] = new PaddleSelectState(),
            });

            Instance = this;
        }

        public override int VirtualWidth => 432;
        public override int VirtualHeight => 243;

        public int PaddleSpeed { get; } = 200;

        public Dictionary<string, Font> Fonts { get; private set; }
        public Dictionary<string, Image> Textures { get; private set; }
        public Dictionary<string, Quad[]> Frames { get; private set; }
        public StateMachine StateMachine { get; }

        // Called just once at the beginning of the game; used to set up
        // game objects, variables, etc. and prepare the game world.
        public override async Task Load()
        {
            // initialize our nice-looking retro text fonts
            Fonts = new Dictionary<string, Font>
            {
                ["small"] = await Graphics.NewFont("fonts/font.ttf", 8),
                ["medium"] = await Graphics.NewFont("fonts/font.ttf", 16),
                ["large"] = await Graphics.NewFont("fonts/font.ttf", 32)
            };
            Graphics.SetFont(Fonts["small"]);

            // load up the graphics we'll be using throughout our states
            Textures = new Dictionary<string, Image>
            {
                ["background"] = await Graphics.NewImage("graphics/background.png"),
                ["main"] = await Graphics.NewImage("graphics/breakout.png"),
                ["arrows"] = await Graphics.NewImage("graphics/arrows.png"),
                ["hearts"] = await Graphics.NewImage("graphics/hearts.png"),
                ["particle"] = await Graphics.NewImage("graphics/particle.png")
            };

            // Quads we will generate for all of our textures; Quads allow us
            // to show only part of a texture and not the entire thing
            var util = new Util();
            Frames = new Dictionary<string, Quad[]>
            {
                ["arrows"] = util.GenerateQuads(Textures["arrows"], 24, 24),
                ["paddles"] = util.GenerateQuadsPaddles(Textures["main"]),
                ["balls"] = util.GenerateQuadsBalls(Textures["main"]),
                ["bricks"] = util.GenerateQuadsBricks(Textures["main"]),
                ["hearts"] = util.GenerateQuads(Textures["hearts"], 10, 9)
            };

            // set up our sound effects; later, we can just index this table and
            // call each entry's `play` method
            Sounds.Add("paddle-hit", await Audio.NewSource("sounds/paddle_hit.wav"));
            Sounds.Add("score", await Audio.NewSource("sounds/score.wav"));
            Sounds.Add("wall-hit", await Audio.NewSource("sounds/wall_hit.wav"));
            Sounds.Add("confirm", await Audio.NewSource("sounds/confirm.wav"));
            Sounds.Add("select", await Audio.NewSource("sounds/select.wav"));
            Sounds.Add("no-select", await Audio.NewSource("sounds/no-select.wav"));
            Sounds.Add("brick-hit-1", await Audio.NewSource("sounds/brick-hit-1.wav"));
            Sounds.Add("brick-hit-2", await Audio.NewSource("sounds/brick-hit-2.wav"));
            Sounds.Add("hurt", await Audio.NewSource("sounds/hurt.wav"));
            Sounds.Add("victory", await Audio.NewSource("sounds/victory.wav"));
            Sounds.Add("recover", await Audio.NewSource("sounds/recover.wav"));
            Sounds.Add("high-score", await Audio.NewSource("sounds/high_score.wav"));
            Sounds.Add("pause", await Audio.NewSource("sounds/pause.wav"));

            Sounds.Add("music", await Audio.NewSource("sounds/music.wav"));

            await StateMachine.Change("start", new Dictionary<string, object> { ["highScores"] = await LoadHighScores() });

            // play our music outside of all states and set it to looping
            Sounds["music"].Looping = true;
            await Sounds["music"].Play();
        }

        // Called every frame, passing in `dt` since the last frame. `dt`
        // is short for `deltaTime` and is measured in seconds. Multiplying
        // this by any changes we wish to make in our game will allow our
        // game to perform consistently across all hardware; otherwise, any
        // changes we make will be applied as fast as possible and will vary
        // across system hardware.
        public async override Task Update(TimeSpan dt)
        {
            // this time, we pass in dt to the state object we're currently using
            await StateMachine.Update(dt);

            Keyboard.Update();
        }

        // Called each frame after update; is responsible simply for
        // drawing all of our game objects and more to the screen.
        public override async Task Draw()
        {
            // begin drawing with push, in our virtual resolution
            Graphics.Begin();

            // background should be drawn regardless of state, scaled to fit our
            // virtual resolution
            var background = Textures["background"];

            Graphics.Draw(background,
                // draw at coordinates 0, 0
                0, 0,
                // scale factors on X and Y axis so it fills the screen
                (double)VirtualWidth / (background.Width - 1), (double)VirtualHeight / (background.Height - 1));

            // use the state machine to defer rendering to the current state we're in
            await StateMachine.Render();

            // display FPS for debugging; simply comment out to remove
            //displayFPS()

            await Graphics.Apply();
        }

        // Loads high scores from a .lst file, saved in LÖVE2D's default save directory in a subfolder
        // called 'breakout'.
        private async Task<ScoreData[]> LoadHighScores()
        {
            FileSystem.SetIdentity("breakout");

            // if the file doesn't exist, initialize it with some default scores
            if (!(await FileSystem.Exists("breakout.lst")))
            {
                var scoresText = new StringBuilder();
                for (var i = 10; i > 0; i--)
                {
                    scoresText.AppendLine("CTO");
                    scoresText.AppendLine($"{i * 1000}");
                }

                await FileSystem.Write("breakout.lst", scoresText.ToString());
            }

            // flag for whether we're reading a name or not
            var name = true;
            string currentName = null;
            int currentScore;
            var counter = 0;

            // initialize scores table with at least 10 blank entries
            var scores = new ScoreData[10];

            // iterate over each line in the file, filling in names and scores
            foreach (var line in await FileSystem.Lines("breakout.lst"))
            {
                if (name)
                {
                    currentName = line;
                }
                else
                {
                    currentScore = int.Parse(line);
                    scores[counter] = new ScoreData(currentName, currentScore);
                    counter++;
                }

                // flip the name flag
                name = !name;
            }

            return scores;
        }

        // Renders hearts based on how much health the player has. First renders
        // full hearts, then empty hearts for however much health we're missing.
        public void RenderHealth(int health)
        {
            // start of our health rendering
            var healthX = VirtualWidth - 100;

            // render health left
            for (var i = 0; i < health; i++)
            {
                Graphics.Draw(Textures["hearts"], Frames["hearts"][0], healthX, 4);
                healthX += 11;
            }

            // render missing health
            for (var i = 0; i < 3 - health; i++)
            {
                Graphics.Draw(Textures["hearts"], Frames["hearts"][1], healthX, 4);
                healthX += 11;
            }
        }

        // Renders the current FPS.
        public void DisplayFps()
        {
            // simple FPS display across all states
            Graphics.SetFont(Fonts["small"]);
            Graphics.SetColor(0, 255, 0);
            // Graphics.Print("FPS: " + Game.Timer.Fps);
        }

        // Simply renders the player's score at the top right, with left-side padding
        // for the score number.
        public void RenderScore(int score)
        {
            Graphics.SetFont(Fonts["small"]);
            Graphics.Print($"Score:", VirtualWidth - 60, 5);
            Graphics.Print($"{score}", VirtualWidth - 50, 5, 40, Alignment.Right);
        }
    }
}

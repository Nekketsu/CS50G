using GameEngine;
using GameEngine.Audio;
using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using GameEngine.Timers;
using Super50BrosGame.States.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Super50BrosGame
{
    // GD50
    // Super Mario Bros. Remake

    // A classic platformer in the style of Super Mario Bros., using a free
    // art pack. Super Mario Bros. was instrumental in the resurgence of video
    // games in the mid-80s, following the infamous crash shortly after the
    // Atari age of the late 70s. The goal is to navigate various levels from
    // a side perspective, where jumping onto enemies inflicts damage and
    // jumping up into blocks typically breaks them or reveals a powerup.

    // Art pack:
    // https://opengameart.org/content/kenney-16x16

    // Music:
    // https://freesound.org/people/Sirkoto51/sounds/393818/
    public class Super50Bros : Game
    {
        public static new Super50Bros Instance { get; private set; }

        public new StatefulKeyboard Keyboard { get; }

        public Super50Bros(IGameLoop gameLoop, IGraphics graphics, IAudio audio, IKeyboard keyboard, IMouse mouse = null, IFileSystem fileSystem = null) : base(nameof(Super50Bros), gameLoop, graphics, audio, keyboard, mouse, fileSystem)
        {
            Keyboard = new StatefulKeyboard(keyboard);

            Instance = this;
        }

        public override int VirtualWidth => 256;
        public override int VirtualHeight => 144;

        // glolbal standared tile size
        public int TileSize => 16;

        // with and height of screen in tiles
        public int ScreenTileWidth => VirtualWidth / TileSize;
        public int ScreenTileHeight => VirtualHeight / TileSize;

        // speed of scrolling background
        public int BackgroundScrollSpeed => 10;

        // number of tiles in each tile set
        public int TileSetWidth => 5;
        public int TileSetHeight => 4;

        // number of tile sets in sheet
        public int TileSetsWide => 6;
        public int TileSetsTall => 10;

        // number of topper sets in sheet
        public int TopperSetsWide => 6;
        public int TopperSetsTall => 18;

        // total number of topper and tile sets
        public int TopperSets => TopperSetsWide * TopperSetsTall;
        public int TileSets => TileSetsWide * TileSetsTall;

        // player walking speed
        public int PlayerWalkSpeed => 60;

        // player jumping velocity
        public int PlayerJumpVelocity => -150;

        // snail movement speed
        public int SnailMoveSpeed => 10;

        // tile IDs
        public int TileIdEmpty => 4;
        public int TileIdGround => 2;

        // table of tiles that should trigger a collision
        public int[] CollidableTiles => new int[] { TileIdGround };

        // game object IDs
        public int[] BushIds { get; } = { 0, 1, 4, 5, 6 };
        public int[] CoinIds { get; } = { 0, 1, 2 };
        public int[] Crates { get; } = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        public int[] Gems { get; } = { 0, 1, 2, 3, 4, 5, 6, 7 };

        public int[] JumpBlocks { get; } = Enumerable.Range(0, 30).ToArray();

        public Dictionary<string, Font> Fonts { get; private set; }
        public Dictionary<string, Image> Textures { get; private set; }
        public Dictionary<string, Quad[]> Frames { get; private set; }
        public Dictionary<string, Quad[][]> FrameSets { get; private set; }
        public StateMachine StateMachine { get; private set; }

        public Timer Timer { get; private set; }

        public override async Task Load()
        {
            Sounds.Add("jump", await Audio.NewSource("sounds/jump.wav"));
            Sounds.Add("death", await Audio.NewSource("sounds/death.wav"));
            Sounds.Add("music", await Audio.NewSource("sounds/music.wav"));
            Sounds.Add("powerup-reveal", await Audio.NewSource("sounds/powerup-reveal.wav"));
            Sounds.Add("pickup", await Audio.NewSource("sounds/pickup.wav"));
            Sounds.Add("empty-block", await Audio.NewSource("sounds/empty-block.wav"));
            Sounds.Add("kill", await Audio.NewSource("sounds/kill.wav"));
            Sounds.Add("kill2", await Audio.NewSource("sounds/kill2.wav"));

            Textures = new Dictionary<string, Image>
            {
                ["tiles"] = await Graphics.NewImage("graphics/tiles.png"),
                ["toppers"] = await Graphics.NewImage("graphics/tile_tops.png"),
                ["bushes"] = await Graphics.NewImage("graphics/bushes_and_cacti.png"),
                ["jump-blocks"] = await Graphics.NewImage("graphics/jump_blocks.png"),
                ["gems"] = await Graphics.NewImage("graphics/gems.png"),
                ["backgrounds"] = await Graphics.NewImage("graphics/backgrounds.png"),
                ["green-alien"] = await Graphics.NewImage("graphics/green_alien.png"),
                ["creatures"] = await Graphics.NewImage("graphics/creatures.png"),
            };

            var util = new Util();
            Frames = new Dictionary<string, Quad[]>
            {
                ["tiles"] = util.GenerateQuads(Textures["tiles"], TileSize, TileSize),
                ["toppers"] = util.GenerateQuads(Textures["toppers"], TileSize, TileSize),
                ["bushes"] = util.GenerateQuads(Textures["bushes"], 16, 16),
                ["jump-blocks"] = util.GenerateQuads(Textures["jump-blocks"], 16, 16),
                ["gems"] = util.GenerateQuads(Textures["gems"], 16, 16),
                ["backgrounds"] = util.GenerateQuads(Textures["backgrounds"], 256, 128),
                ["green-alien"] = util.GenerateQuads(Textures["green-alien"], 16, 20),
                ["creatures"] = util.GenerateQuads(Textures["creatures"], 16, 16)
            };

            // these need to be added after gFrames is initialized because they refer to gFrames from within
            FrameSets = new Dictionary<string, Quad[][]>
            {
                ["tilesets"] = util.GenerateTileSets(Frames["tiles"], TileSetsWide, TileSetsTall, TileSetWidth, TileSetHeight),
                ["toppersets"] = util.GenerateTileSets(Frames["toppers"], TopperSetsWide, TopperSetsTall, TileSetWidth, TileSetHeight)
            };

            Fonts = new Dictionary<string, Font>
            {
                ["small"] = await Graphics.NewFont("fonts/font.ttf", 8),
                ["medium"] = await Graphics.NewFont("fonts/font.ttf", 16),
                ["large"] = await Graphics.NewFont("fonts/font.ttf", 32),
                ["title"] = await Graphics.NewFont("fonts/ArcadeAlternate.ttf", 32),
            };

            Timer = new Timer();

            StateMachine = new StateMachine(new Dictionary<string, State>
            {
                ["start"] = new StartState(),
                ["play"] = new PlayState()
            });
            await StateMachine.Change("start");

            Sounds["music"].Looping = true;
            Sounds["music"].Volume = 0.5;
            await Sounds["music"].Play();
        }

        public override void KeyPressed(Key key)
        {
            if (key == Key.Escape)
            {
                // Quit
            }
        }

        public override async Task Update(TimeSpan dt)
        {
            await StateMachine.Update(dt);
            await Timer.Update(dt);
            Keyboard.Update();
        }

        public override async Task Draw()
        {
            Graphics.Begin();

            await StateMachine.Render();

            await Graphics.Apply();
        }
    }
}

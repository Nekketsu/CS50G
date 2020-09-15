using GameEngine;
using GameEngine.Audio;
using GameEngine.Events;
using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using GameEngine.Timers;
using LegendOf50Game.States.Game;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegendOf50Game
{
    public class LegendOf50 : Game
    {
        public static new LegendOf50 Instance { get; private set; }

        public new StatefulKeyboard Keyboard { get; }
        public Event Event { get; set; }


        public LegendOf50(IGameLoop gameLoop, IGraphics graphics, IAudio audio, IKeyboard keyboard, IMouse mouse = null, IFileSystem fileSystem = null) : base(nameof(LegendOf50), gameLoop, graphics, audio, keyboard, mouse, fileSystem)
        {
            Keyboard = new StatefulKeyboard(keyboard);

            Instance = this;
        }

        public override int VirtualWidth => 384;
        public override int VirtualHeight => 216;

        public int TileSize => 16;

        // entity constants
        public int PlayerWalkSpeed => 60;

        // map constants
        public int MapWidth => VirtualWidth / TileSize - 2;
        public int MapHeight => VirtualHeight / TileSize - 2;

        public int MapRenderOffsetX => (VirtualWidth - (MapWidth * TileSize)) / 2;
        public int MapRenderOffsetY => (VirtualHeight - (MapHeight * TileSize)) / 2;

        // tile IDs
        public int TileTopLeftCorner => 3;
        public int TileTopRightCorner => 4;
        public int TileBottomLeftCorner => 22;
        public int TileBottomRightCorner => 23;

        public int TileEmpty => 19;

        public int[] TileFloors { get; } =
        {
            6, 7, 8, 9, 10, 11, 12,
            25, 26, 27, 28, 29, 30, 31,
            44, 45, 46, 47, 48, 49, 50,
            63, 64, 65, 66, 67, 68, 69,
            87, 88, 106, 107
        };

        public int[] TileTopWalls { get; } = { 57, 58, 59 };
        public int[] TileBottomWalls { get; } = { 78, 79, 80 };
        public int[] TileLeftWalls { get; } = { 76, 95, 114 };
        public int[] TileRightWalls { get; } = { 77, 96, 115 };


        public Dictionary<string, Font> Fonts { get; private set; }
        public Dictionary<string, Image> Textures { get; private set; }
        public Dictionary<string, Quad[]> Frames { get; private set; }
        public Dictionary<string, Quad[][]> FrameSets { get; private set; }
        public StateMachine StateMachine { get; private set; }

        public Timer Timer { get; private set; }

        public Dictionary<string, GameObjectDef> GameObjectDefs { get; private set; }
        public Dictionary<string, EntityDef> EntityDefs { get; private set; }

        public override async Task Load()
        {
            Textures = new Dictionary<string, Image>
            {
                ["tiles"] = await Graphics.NewImage("graphics/tilesheet.png"),
                ["background"] = await Graphics.NewImage("graphics/background.png"),
                ["character-walk"] = await Graphics.NewImage("graphics/character_walk.png"),
                ["character-swing-sword"] = await Graphics.NewImage("graphics/character_swing_sword.png"),
                ["hearts"] = await Graphics.NewImage("graphics/hearts.png"),
                ["switches"] = await Graphics.NewImage("graphics/switches.png"),
                ["entities"] = await Graphics.NewImage("graphics/entities.png"),
            };

            var utils = new Utils();
            Frames = new Dictionary<string, Quad[]>
            {
                ["tiles"] = utils.GenerateQuads(Textures["tiles"], 16, 16),
                ["character-walk"] = utils.GenerateQuads(Textures["character-walk"], 16, 32),
                ["character-swing-sword"] = utils.GenerateQuads(Textures["character-swing-sword"], 32, 32),
                ["entities"] = utils.GenerateQuads(Textures["entities"], 16, 16),
                ["hearts"] = utils.GenerateQuads(Textures["hearts"], 16, 16),
                ["switches"] = utils.GenerateQuads(Textures["switches"], 16, 18),
            };

            Fonts = new Dictionary<string, Font>
            {
                ["small"] = await Graphics.NewFont("fonts/font.ttf", 8),
                ["medium"] = await Graphics.NewFont("fonts/font.ttf", 16),
                ["large"] = await Graphics.NewFont("fonts/font.ttf", 32),
                ["gothic-medium"] = await Graphics.NewFont("fonts/GothicPixels.ttf", 16),
                ["gothic-large"] = await Graphics.NewFont("fonts/GothicPixels.ttf", 32),
                ["zelda"] = await Graphics.NewFont("fonts/zelda.otf", 64),
                ["zelda-small"] = await Graphics.NewFont("fonts/zelda.otf", 32),
            };

            Sounds.Add("music", await Audio.NewSource("sounds/music.mp3"));
            Sounds.Add("sword", await Audio.NewSource("sounds/sword.wav"));
            Sounds.Add("hit-enemy", await Audio.NewSource("sounds/hit_enemy.wav"));
            Sounds.Add("hit-player", await Audio.NewSource("sounds/hit_player.wav"));
            Sounds.Add("door", await Audio.NewSource("sounds/door.wav"));

            var gameObjectDefs = new GameObjectDefs();
            GameObjectDefs = gameObjectDefs.Defs;

            var entityDefs = new EntityDefs();
            EntityDefs = entityDefs.Defs;

            Graphics.SetFont(Fonts["small"]);

            Timer = new Timer();
            Event = new Event();

            StateMachine = new StateMachine(new Dictionary<string, State>
            {
                ["start"] = new StartState(),
                ["play"] = new PlayState(),
                ["game-over"] = new GameOverState()
            });
            await StateMachine.Change("start");

            Sounds["music"].Looping = true;
            await Sounds["music"].Play();
        }

        public override async Task Update(TimeSpan dt)
        {
            await Timer.Update(dt);
            await StateMachine.Update(dt);

            Keyboard.Update();
        }

        public override async Task Draw()
        {
            Graphics.Begin();
            Graphics.Clear(0, 0, 0);
            await StateMachine.Render();
            await Graphics.Apply();
        }
    }
}

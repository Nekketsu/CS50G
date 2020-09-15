using GameEngine.States;
using Super50BrosGame.States.Entity;
using Super50BrosGame.States.Entity.Snail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Super50BrosGame.States.Game
{
    public class PlayState : State
    {
        public double CamX { get; private set; }
        public double CamY { get; private set; }
        public GameLevel Level { get; private set; }
        public TileMap TileMap { get; private set; }
        public int Background { get; private set; }
        public double BackgroundX { get; private set; }

        public bool GravityOn { get; private set; }
        public int GravityAmount { get; private set; }
        public Player Player { get; private set; }

        public override async Task Enter(Dictionary<string, object> parameters)
        {
            var levelMaker = new LevelMaker();

            CamX = 0;
            CamY = 0;
            Level = levelMaker.Generate(100, 10);
            TileMap = Level.TileMap;
            Background = Super50Bros.Instance.Random.Next(3);
            BackgroundX = 0;

            GravityOn = true;
            GravityAmount = 6 * 60; // ~60 FPS

            Player = new Player(
                0, 0,
                16, 20,
                "green-alien",
                null,
                TileMap,
                Level
            );

            Player.StateMachine = new StateMachine(new Dictionary<string, State>
            {
                ["idle"] = new PlayerIdleState(Player),
                ["walking"] = new PlayerWalkingState(Player),
                ["jump"] = new PlayerJumpState(Player, GravityAmount),
                ["falling"] = new PlayerFallingState(Player, GravityAmount)
            });

            await SpawnEnemies();

            await Player.ChangeState("falling");
        }

        public override async Task Update(TimeSpan dt)
        {
            // remove any nulls from pickups, etc.
            Level.Clear();

            // update polayer and level
            await Player.Update(dt);
            await Level.Update(dt);

            // constrain player X no matter which state
            if (Player.X <= 0)
            {
                Player.X = 0;
            }
            else if (Player.X > Super50Bros.Instance.TileSize * TileMap.Width - Player.Width)
            {
                Player.X = Super50Bros.Instance.TileSize * TileMap.Width - Player.Width;
            }

            UpdateCamera();
        }

        public override Task Render()
        {
            Super50Bros.Instance.Graphics.Push();
            Super50Bros.Instance.Graphics.Draw(Super50Bros.Instance.Textures["backgrounds"], Super50Bros.Instance.Frames["backgrounds"][Background], Math.Floor(-BackgroundX), 0);
            Super50Bros.Instance.Graphics.Draw(Super50Bros.Instance.Textures["backgrounds"], Super50Bros.Instance.Frames["backgrounds"][Background], Math.Floor(-BackgroundX),
                Super50Bros.Instance.Textures["backgrounds"].Height / 3 * 2, 1, -1);
            Super50Bros.Instance.Graphics.Draw(Super50Bros.Instance.Textures["backgrounds"], Super50Bros.Instance.Frames["backgrounds"][Background], Math.Floor(-BackgroundX + 256), 0);
            Super50Bros.Instance.Graphics.Draw(Super50Bros.Instance.Textures["backgrounds"], Super50Bros.Instance.Frames["backgrounds"][Background], Math.Floor(-BackgroundX + 256),
                Super50Bros.Instance.Textures["backgrounds"].Height / 3 * 2, 1, -1);

            // translate the entire view of the scene to emulate a camera
            Super50Bros.Instance.Graphics.Translate((int)-Math.Floor(CamX), (int)-Math.Floor(CamY));

            Level.Render((int)CamX, (int)CamY);

            Player.Render((int)CamX, (int)CamY);
            Super50Bros.Instance.Graphics.Pop();

            // render score
            Super50Bros.Instance.Graphics.SetFont(Super50Bros.Instance.Fonts["medium"]);
            Super50Bros.Instance.Graphics.SetColor(0, 0, 0, 255);
            Super50Bros.Instance.Graphics.Print($"{Player.Score}", 5, 5);
            Super50Bros.Instance.Graphics.SetColor(255, 255, 255, 255);
            Super50Bros.Instance.Graphics.Print($"{Player.Score}", 4, 4);

            return Task.CompletedTask;
        }

        public void UpdateCamera()
        {
            // clamp movement of the camera's X between 0 and the map bounds - virtual width,
            // setting it half the screen to the left of the player so they are in the center
            CamX = Math.Max(0,
                Math.Min(Super50Bros.Instance.TileSize * TileMap.Width - Super50Bros.Instance.VirtualWidth,
                Player.X - (Super50Bros.Instance.VirtualWidth / 2 - 8)));

            // adjust background X to move a third the rate of the camera for parallax
            BackgroundX = (CamX / 3) % 256;
        }

        // Adds a series of enemies to the level randomly.
        public async Task SpawnEnemies()
        {
            // spawn snails in the level
            for (var x = 0; x < TileMap.Width; x++)
            {
                // flag for whether there's ground on this column of the level
                var groundFound = false;

                for (var y = 0; y < TileMap.Height; y++)
                {
                    if (!groundFound)
                    {
                        if (TileMap.Tiles[y, x].Id == Super50Bros.Instance.TileIdGround)
                        {
                            groundFound = true;

                            // random chance, 1, in 20
                            if (Super50Bros.Instance.Random.Next(20) == 0)
                            {
                                // instantiate snail, ceclaring in advane so we can pass it into state machine
                                var snail = new Snail(
                                    "creatures",
                                    x * Super50Bros.Instance.TileSize,
                                    (y - 1) * Super50Bros.Instance.TileSize + 2,
                                    16,
                                    16,
                                    null,
                                    TileMap,
                                    Level);

                                snail.StateMachine = new StateMachine(new Dictionary<string, State>
                                {
                                    ["idle"] = new SnailIdleState(TileMap, Player, snail),
                                    ["moving"] = new SnailMovingState(TileMap, Player, snail),
                                    ["chasing"] = new SnailChasingState(TileMap, Player, snail)
                                });
                                await snail.ChangeState("idle", new Dictionary<string, object>
                                {
                                    ["wait"] = TimeSpan.FromSeconds(Super50Bros.Instance.Random.Next(5) + 1)
                                });
                                Level.Entities.Add(snail);
                            }
                        }
                    }
                }
            }
        }
    }
}

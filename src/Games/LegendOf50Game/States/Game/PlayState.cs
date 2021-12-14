using GameEngine.Input;
using GameEngine.States;
using LegendOf50Game.States.Entities.Player;
using LegendOf50Game.World;

namespace LegendOf50Game.States.Game
{
    // PlayState Class
    public class PlayState : State
    {
        public Player Player { get; private set; }
        public Dungeon Dungeon { get; private set; }
        public Room CurrentRoom { get; private set; }

        public override async Task Enter(Dictionary<string, object> parameters)
        {
            Player = new Player(
                LegendOf50.Instance.EntityDefs["player"].Animations,

                LegendOf50.Instance.VirtualWidth / 2 - 8,
                LegendOf50.Instance.VirtualHeight / 2 - 11,

                16,
                22,

                LegendOf50.Instance.EntityDefs["player"].WalkSpeed,

                // one heart == 2 health
                6,

                // rendering and collision offset for spaced sprites
                offsetY: 5
                );

            Dungeon = await Dungeon.Create(Player);
            CurrentRoom = await Room.Create(Player);

            Player.StateMachine = new StateMachine(new Dictionary<string, State>
            {
                ["walk"] = new PlayerWalkState(Player, Dungeon),
                ["idle"] = new PlayerIdleState(Player),
                ["swing-sword"] = new PlayerSwingSwordState(Player, Dungeon)
            });

            await Player.ChangeState("idle");
        }

        public override async Task Update(TimeSpan dt)
        {
            if (LegendOf50.Instance.Keyboard.WasPressed(Key.Escape))
            {
                // Quit
            }

            await Dungeon.Update(dt);
        }

        public override async Task Render()
        {
            // render dungeon and all entities separate from herats GUI
            LegendOf50.Instance.Graphics.Push();
            await Dungeon.Render();
            LegendOf50.Instance.Graphics.Pop();

            // draw player hearts, top of screen
            var healthLeft = Player.Health;
            var heartFrame = 1;

            for (var i = 1; i <= 3; i++)
            {
                if (healthLeft > 1)
                {
                    heartFrame = 4;
                }
                else if (healthLeft == 1)
                {
                    heartFrame = 2;
                }
                else
                {
                    heartFrame = 0;
                }

                LegendOf50.Instance.Graphics.Draw(LegendOf50.Instance.Textures["hearts"], LegendOf50.Instance.Frames["hearts"][heartFrame],
                    (i - 1) * (LegendOf50.Instance.TileSize + 1), 2);

                healthLeft -= 2;
            }
        }
    }
}

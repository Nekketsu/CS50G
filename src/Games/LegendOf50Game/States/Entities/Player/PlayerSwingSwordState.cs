using GameEngine.Input;
using GameEngine.States;
using LegendOf50Game.World;

namespace LegendOf50Game.States.Entities.Player
{
    // PlayerSwingSwordState Class
    public class PlayerSwingSwordState : State
    {
        public LegendOf50Game.Player Player { get; }
        public Dungeon Dungeon { get; }
        public Hitbox SwordHitbox { get; private set; }

        public PlayerSwingSwordState(LegendOf50Game.Player player, Dungeon dungeon)
        {
            Player = player;
            Dungeon = dungeon;
        }

        public override async Task Enter(Dictionary<string, object> parameters)
        {
            // render offset for spaced character sprite
            Player.OffsetX = 5;
            Player.OffsetY = 8;

            // create hitbox based on where the player is and facing
            var direction = Player.Direction;

            var hitboxX = 0;
            var hitboxY = 0;
            var hitboxWidth = 0;
            var hitboxHeight = 0;

            switch (direction)
            {
                case Direction.Left:
                    hitboxWidth = 8;
                    hitboxHeight = 16;
                    hitboxX = (int)(Player.X - hitboxWidth);
                    hitboxY = (int)(Player.Y + 2);
                    break;
                case Direction.Right:
                    hitboxWidth = 8;
                    hitboxHeight = 16;
                    hitboxX = (int)(Player.X + Player.Width);
                    hitboxY = (int)(Player.Y + 2);
                    break;
                case Direction.Up:
                    hitboxWidth = 16;
                    hitboxHeight = 8;
                    hitboxX = (int)Player.X;
                    hitboxY = (int)(Player.Y - hitboxHeight);
                    break;
                case Direction.Down:
                    hitboxWidth = 16;
                    hitboxHeight = 8;
                    hitboxX = (int)Player.X;
                    hitboxY = (int)(Player.Y + Player.Height);
                    break;
            }

            SwordHitbox = new Hitbox(hitboxX, hitboxY, hitboxWidth, hitboxHeight);
            Player.ChangeAnimation($"sword-{Player.Direction.ToString().ToLower()}");


            await LegendOf50.Instance.Sounds["sword"].Stop();
            await LegendOf50.Instance.Sounds["sword"].Play();

            // restart sword swing animation
            Player.CurrentAnimation.Refresh();
        }

        public override async Task Update(TimeSpan dt)
        {
            // check if hitbox collides with any entities in the scene
            foreach (var entity in Dungeon.CurrentRoom.Entities)
            {
                if (entity.Collides(SwordHitbox))
                {
                    entity.Damage(1);
                    await LegendOf50.Instance.Sounds["hit-enemy"].Play();
                }
            }

            if (Player.CurrentAnimation.TimesPlayed > 0)
            {
                Player.CurrentAnimation.TimesPlayed = 0;
                await Player.ChangeState("idle");
            }

            if (LegendOf50.Instance.Keyboard.WasPressed(Key.Space))
            {
                await Player.ChangeState("swing-sword");
            }
        }

        public override Task Render()
        {
            var anim = Player.CurrentAnimation;
            LegendOf50.Instance.Graphics.Draw(LegendOf50.Instance.Textures[anim.Texture], LegendOf50.Instance.Frames[anim.Texture][anim.GetCurrentFrame()],
                (int)(Player.X - Player.OffsetX), (int)(Player.Y - Player.OffsetY));

            // debug for player and hurtbox collision rects
            //LegendOf50.Instance.Graphics.SetColor(255, 0, 255, 255);
            //LegendOf50.Instance.Graphics.Rectangle(DrawMode.Line, Player.X, Player.Y, Player.Width, Player.Height);
            //LegendOf50.Instance.Graphics.Rectangle(DrawMode.Line, SwordHitbox.X, SwordHitbox.Y,
            //    SwordHitbox.Width, SwordHitbox.Height);
            //LegendOf50.Instance.Graphics.SetColor(255, 255, 255, 255);

            return Task.CompletedTask;
        }
    }
}

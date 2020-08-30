using GameEngine.Input;
using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Super50BrosGame.States.Entity
{
    // PlayerWalkingState Class
    public class PlayerWalkingState : State
    {
        public Player Player { get; }
        public Animation Animation { get; private set; }

        public PlayerWalkingState(Player player)
        {
            Player = player;
        }

        public override Task Enter(Dictionary<string, object> parameters)
        {
            Animation = new Animation(new int[] { 9, 10 }, TimeSpan.FromSeconds(0.1));
            Player.CurrentAnimation = Animation;

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            Player.CurrentAnimation.Update(dt);

            // idle if we're not pressing anything at all
            if (!Super50Bros.Instance.Keyboard.IsDown(Key.Left) && !Super50Bros.Instance.Keyboard.IsDown(Key.Right))
            {
                await Player.ChangeState("idle");
            }
            else
            {
                var tileBottomLeft = Player.Map.PointToTile(Player.X + 1, Player.Y + Player.Height);
                var tileBottomRigth = Player.Map.PointToTile(Player.X + Player.Width - 1, Player.Y + Player.Height);

                // temporarily shift player down a pixel to test for game objects beneath
                Player.Y++;

                var collidedObjects = Player.CheckObjectCollisions();

                Player.Y--;

                // check to see whether there are any tiles beneath us
                if (!collidedObjects.Any() && (tileBottomLeft != null && tileBottomRigth != null) && (!tileBottomLeft.Collidable() && !tileBottomRigth.Collidable()))
                {
                    Player.Dy = 0;
                    await Player.ChangeState("falling");
                }
                else if (Super50Bros.Instance.Keyboard.IsDown(Key.Left))
                {
                    Player.X -= Super50Bros.Instance.PlayerWalkSpeed * dt.TotalSeconds;
                    Player.Direction = Direction.Left;
                    Player.CheckLeftCollisions(dt);
                }
                else if (Super50Bros.Instance.Keyboard.IsDown(Key.Right))
                {
                    Player.X += Super50Bros.Instance.PlayerWalkSpeed * dt.TotalSeconds;
                    Player.Direction = Direction.Right;
                    Player.CheckRightCollisions(dt);
                }

                // check if we've collided with any entities and die if so
                foreach (var entity in Player.Level.Entities)
                {
                    if (entity.Collides(Player))
                    {
                        await Super50Bros.Instance.Sounds["death"].Play();
                        await Super50Bros.Instance.StateMachine.Change("start");
                    }
                }

                if (Super50Bros.Instance.Keyboard.WasPressed(Key.Space))
                {
                    await Player.ChangeState("jump");
                }
            }
        }
    }
}

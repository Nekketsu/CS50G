using GameEngine.Input;
using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Super50BrosGame.States.Entity
{
    public class PlayerJumpState : State
    {
        public Player Player { get; }
        public double Gravity { get; }
        public Animation Animation { get; private set; }

        public PlayerJumpState(Player player, double gravity)
        {
            Player = player;
            Gravity = gravity;
        }

        public override async Task Enter(Dictionary<string, object> parameters)
        {
            Animation = new Animation(new int[] { 2 }, TimeSpan.FromSeconds(1));
            Player.CurrentAnimation = Animation;

            await Super50Bros.Instance.Sounds["jump"].Play();
            Player.Dy = Super50Bros.Instance.PlayerJumpVelocity;
        }

        public override async Task Update(TimeSpan dt)
        {
            Player.CurrentAnimation.Update(dt);
            Player.Dy += Gravity * dt.TotalSeconds;
            Player.Y += Player.Dy * dt.TotalSeconds;

            // go into the falling state when y velocity is positive
            if (Player.Dy >= 0)
            {
                await Player.ChangeState("falling");
            }

            Player.Y += Player.Dy * dt.TotalSeconds;

            // look at two tiles above our head and check for collisions; 3 pixels of leeway for getting through gaps
            var tileLeft = Player.Map.PointToTile(Player.X + 3, Player.Y);
            var tileRight = Player.Map.PointToTile(Player.X + Player.Width - 3, Player.Y);

            // if we get a collision up top, go into the falling state immediately
            if ((tileLeft != null && tileRight != null) && (tileLeft.Collidable() || tileRight.Collidable()))
            {
                Player.Dy = 0;
                await Player.ChangeState("falling");
            }

            // else test our sides for blocks
            else if (Super50Bros.Instance.Keyboard.IsDown(Key.Left))
            {
                Player.Direction = Direction.Left;
                Player.X -= Super50Bros.Instance.PlayerWalkSpeed * dt.TotalSeconds;
                Player.CheckLeftCollisions(dt);
            }
            else if (Super50Bros.Instance.Keyboard.IsDown(Key.Right))
            {
                Player.Direction = Direction.Right;
                Player.X += Super50Bros.Instance.PlayerWalkSpeed * dt.TotalSeconds;
                Player.CheckRightCollisions(dt);
            }

            // check if we've collided with any collidable game objects
            foreach (var @object in Player.Level.Objects.ToArray())
            {
                if (@object.Collides(Player))
                {
                    if (@object.Solid == true)
                    {
                        @object.OnCollide(@object);

                        Player.Y = @object.Y + @object.Height;
                        Player.Dy = 0;
                        await Player.ChangeState("falling");
                    }
                    else if (@object.Consumable == true)
                    {
                        @object.OnConsume(Player, @object);
                        Player.Level.Objects.Remove(@object);
                    }
                }
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
        }
    }
}

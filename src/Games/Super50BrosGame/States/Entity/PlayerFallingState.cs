using GameEngine.Input;
using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Super50BrosGame.States.Entity
{
    // PlayerFallingState Class
    public class PlayerFallingState : State
    {
        public Player Player { get; }
        public double Gravity { get; }
        public Animation Animation { get; private set; }

        public PlayerFallingState(Player player, double gravity)
        {
            Player = player;
            Gravity = gravity;
        }

        public override Task Enter(Dictionary<string, object> parameters)
        {
            Animation = new Animation(new int[] { 2 }, TimeSpan.FromSeconds(1));
            Player.CurrentAnimation = Animation;

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            Player.CurrentAnimation.Update(dt);
            Player.Dy += Gravity * dt.TotalSeconds;
            Player.Y += Player.Dy * dt.TotalSeconds;

            // look at two tiles below our feet and check for collisions
            var tileBottomLeft = Player.Map.PointToTile(Player.X + 1, Player.Y + Player.Height);
            var tileBottomRight = Player.Map.PointToTile(Player.X + Player.Width - 1, Player.Y + Player.Height);

            // if we get a collision beneath us, go into either walking or idle
            if ((tileBottomLeft != null && tileBottomRight != null) && (tileBottomLeft.Collidable() || tileBottomRight.Collidable()))
            {
                Player.Dy = 0;

                // set the player to be walking or idle on landing depending on input
                if (Super50Bros.Instance.Keyboard.IsDown(Key.Left) || Super50Bros.Instance.Keyboard.IsDown(Key.Right))
                {
                    await Player.ChangeState("walking");
                }
                else
                {
                    await Player.ChangeState("idle");
                }

                Player.Y = tileBottomLeft.Y * Super50Bros.Instance.TileSize - Player.Height;
            }
            // go back to start if we fall below the map boundary
            else if (Player.Y > Super50Bros.Instance.VirtualHeight)
            {
                await Super50Bros.Instance.Sounds["death"].Play();
                await Super50Bros.Instance.StateMachine.Change("start");
            }

            // check side collisions and reset position
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
                        Player.Dy = 0;
                        Player.Y = @object.Y - Player.Height;

                        if (Super50Bros.Instance.Keyboard.IsDown(Key.Left) || Super50Bros.Instance.Keyboard.IsDown(Key.Right))
                        {
                            await Player.ChangeState("walking");
                        }
                        else
                        {
                            await Player.ChangeState("idle");
                        }
                    }
                    else if (@object.Consumable == true)
                    {
                        @object.OnConsume(Player, @object);
                        Player.Level.Objects.Remove(@object);
                    }
                }
            }

            // check if we've collided with any entities and kill them if so
            foreach (var entity in Player.Level.Entities.ToArray())
            {
                if (entity.Collides(Player))
                {
                    await Super50Bros.Instance.Sounds["kill"].Play();
                    await Super50Bros.Instance.Sounds["kill2"].Play();
                    Player.Score += 100;
                    Player.Level.Entities.Remove(entity);
                }
            }
        }
    }
}

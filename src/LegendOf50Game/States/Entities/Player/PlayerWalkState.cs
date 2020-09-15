using GameEngine.Input;
using LegendOf50Game.World;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegendOf50Game.States.Entities.Player
{
    // PlayerWalkState Class
    public class PlayerWalkState : EntityWalkState
    {
        public PlayerWalkState(Entity entity, Dungeon dungeon = null) : base(entity, dungeon)
        {
        }

        public override async Task Enter(Dictionary<string, object> parameters)
        {
            await base.Enter(parameters);

            // render offset for spaced character sprite
            Entity.OffsetY = 5;
            Entity.OffsetX = 0;
        }

        public override async Task Update(TimeSpan dt)
        {
            if (LegendOf50.Instance.Keyboard.IsDown(Key.Left))
            {
                Entity.Direction = Direction.Left;
                Entity.ChangeAnimation("walk-left");
            }
            else if (LegendOf50.Instance.Keyboard.IsDown(Key.Right))
            {
                Entity.Direction = Direction.Right;
                Entity.ChangeAnimation("walk-right");
            }
            else if (LegendOf50.Instance.Keyboard.IsDown(Key.Up))
            {
                Entity.Direction = Direction.Up;
                Entity.ChangeAnimation("walk-up");
            }
            else if (LegendOf50.Instance.Keyboard.IsDown(Key.Down))
            {
                Entity.Direction = Direction.Down;
                Entity.ChangeAnimation("walk-down");
            }
            else
            {
                await Entity.ChangeState("idle");
            }

            if (LegendOf50.Instance.Keyboard.WasPressed(Key.Space))
            {
                await Entity.ChangeState("swing-sword");
            }

            // perform base collision detection against walls
            await base.Update(dt);

            // if we bumped something when checking collision, check any object collisions
            if (Bumped)
            {
                if (Entity.Direction == Direction.Left)
                {
                    // temporarily adjust position
                    Entity.X -= LegendOf50.Instance.PlayerWalkSpeed * dt.TotalSeconds;

                    foreach (var doorway in Dungeon.CurrentRoom.Doorways)
                    {
                        if (Entity.Collides(doorway) && doorway.Open)
                        {
                            // shift entity to center of door to avoid phasing through wall
                            Entity.Y = doorway.Y + 4;
                            await LegendOf50.Instance.Event.Dispatch("shift-left");
                            break;
                        }
                    }

                    // readjust
                    Entity.X += LegendOf50.Instance.PlayerWalkSpeed * dt.TotalSeconds;
                }
                else if (Entity.Direction == Direction.Right)
                {
                    // temporarily adjust position
                    Entity.X += LegendOf50.Instance.PlayerWalkSpeed * dt.TotalSeconds;

                    foreach (var doorway in Dungeon.CurrentRoom.Doorways)
                    {
                        if (Entity.Collides(doorway) && doorway.Open)
                        {
                            // shift entity to center of door to avoid phasing through wall
                            Entity.Y = doorway.Y + 4;
                            await LegendOf50.Instance.Event.Dispatch("shift-right");
                            break;
                        }
                    }

                    // readjust
                    Entity.X -= LegendOf50.Instance.PlayerWalkSpeed * dt.TotalSeconds;
                }
                else if (Entity.Direction == Direction.Up)
                {
                    // temporarily adjust position
                    Entity.Y -= LegendOf50.Instance.PlayerWalkSpeed * dt.TotalSeconds;

                    foreach (var doorway in Dungeon.CurrentRoom.Doorways)
                    {
                        if (Entity.Collides(doorway) && doorway.Open)
                        {
                            // shift entity to center of door to avoid phasing through wall
                            Entity.Y = doorway.X + 8;
                            await LegendOf50.Instance.Event.Dispatch("shift-up");
                            break;
                        }
                    }

                    // readjust
                    Entity.Y += LegendOf50.Instance.PlayerWalkSpeed * dt.TotalSeconds;
                }
                else
                {
                    // temporarily adjust position
                    Entity.Y += LegendOf50.Instance.PlayerWalkSpeed * dt.TotalSeconds;

                    foreach (var doorway in Dungeon.CurrentRoom.Doorways)
                    {
                        if (Entity.Collides(doorway) && doorway.Open)
                        {
                            // shift entity to center of door to avoid phasing through wall
                            Entity.Y = doorway.X + 8;
                            await LegendOf50.Instance.Event.Dispatch("shift-down");
                            break;
                        }
                    }

                    // readjust
                    Entity.Y -= LegendOf50.Instance.PlayerWalkSpeed * dt.TotalSeconds;
                }
            }
        }
    }
}

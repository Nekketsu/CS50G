using GameEngine.States;
using LegendOf50Game.World;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegendOf50Game.States.Entities
{
    // EntityWalkState Class
    public class EntityWalkState : State
    {
        public Entity Entity { get; }

        public Dungeon Dungeon { get; }

        public TimeSpan MoveDuration { get; private set; }
        public TimeSpan MovementTimer { get; private set; }

        public bool Bumped { get; private set; }

        public EntityWalkState(Entity entity, Dungeon dungeon = null)
        {
            Entity = entity;
            Dungeon = dungeon;

            Entity.ChangeAnimation("walk-down");
        }

        public override Task Enter(Dictionary<string, object> parameters)
        {
            Entity.ChangeAnimation("walk-down");

            // used for AI control
            MoveDuration = TimeSpan.Zero;
            MovementTimer = TimeSpan.Zero;

            // keeps track of whether we just hit a wall
            Bumped = false;

            return Task.CompletedTask;
        }

        public override Task Update(TimeSpan dt)
        {
            // assume we didn't hit a wall
            Bumped = false;

            if (Entity.Direction == Direction.Left)
            {
                Entity.X -= Entity.WalkSpeed * dt.TotalSeconds;

                if (Entity.X <= LegendOf50.Instance.MapRenderOffsetX + LegendOf50.Instance.TileSize)
                {
                    Entity.X = LegendOf50.Instance.MapRenderOffsetX + LegendOf50.Instance.TileSize;
                    Bumped = true;
                }
            }
            else if (Entity.Direction == Direction.Right)
            {
                Entity.X += Entity.WalkSpeed * dt.TotalSeconds;

                if (Entity.X + Entity.Width >= LegendOf50.Instance.VirtualWidth - LegendOf50.Instance.TileSize * 2)
                {
                    Entity.X = LegendOf50.Instance.VirtualWidth - LegendOf50.Instance.TileSize * 2 - Entity.Width;
                    Bumped = true;
                }
            }
            else if (Entity.Direction == Direction.Up)
            {
                Entity.Y -= Entity.WalkSpeed * dt.TotalSeconds;

                if (Entity.Y <= LegendOf50.Instance.MapRenderOffsetY + LegendOf50.Instance.TileSize - Entity.Height / 2)
                {
                    Entity.Y = LegendOf50.Instance.MapRenderOffsetY + LegendOf50.Instance.TileSize - Entity.Height / 2;
                    Bumped = true;
                }
            }
            else if (Entity.Direction == Direction.Down)
            {
                Entity.Y += Entity.WalkSpeed * dt.TotalSeconds;

                var bottomEdge = LegendOf50.Instance.VirtualHeight - (LegendOf50.Instance.VirtualHeight - LegendOf50.Instance.MapHeight * LegendOf50.Instance.TileSize)
                        + LegendOf50.Instance.MapRenderOffsetY - LegendOf50.Instance.TileSize;

                if (Entity.Y + Entity.Height >= bottomEdge)
                {
                    Entity.Y = bottomEdge - Entity.Height;
                    Bumped = true;
                }
            }

            return Task.CompletedTask;
        }

        public override async Task ProcessAI(Dictionary<string, object> parameters, TimeSpan dt)
        {
            var room = (Room)parameters["room"];

            if (MoveDuration == TimeSpan.Zero || Bumped)
            {
                // set an initial move duration and direction
                MoveDuration = TimeSpan.FromSeconds(LegendOf50.Instance.Random.Next(5) + 1);
                Entity.Direction = (Direction)LegendOf50.Instance.Random.Next(Enum.GetValues(typeof(Direction)).Length);
                Entity.ChangeAnimation($"walk-{Entity.Direction.ToString().ToLower()}");
            }
            else if (MovementTimer > MoveDuration)
            {
                MovementTimer = TimeSpan.Zero;

                // change to go idle
                if (LegendOf50.Instance.Random.Next(3) == 0)
                {
                    await Entity.ChangeState("idle");
                }
                else
                {
                    MoveDuration = TimeSpan.FromSeconds(LegendOf50.Instance.Random.Next(5) + 1);
                    Entity.Direction = (Direction)LegendOf50.Instance.Random.Next(Enum.GetValues(typeof(Direction)).Length);
                    Entity.ChangeAnimation($"walk-{Entity.Direction.ToString().ToLower()}");
                }
            }

            MovementTimer += dt;
        }

        public override Task Render()
        {
            var anim = Entity.CurrentAnimation;
            LegendOf50.Instance.Graphics.Draw(LegendOf50.Instance.Textures[anim.Texture], LegendOf50.Instance.Frames[anim.Texture][anim.GetCurrentFrame()],
                (int)(Entity.X - Entity.OffsetX), (int)(Entity.Y - Entity.OffsetY));

            //LegendOf50.Instance.Graphics.SetColor(255, 0, 255, 255);
            //LegendOf50.Instance.Graphics.Rectangle(DrawMode.Line, Entity.X, Entity.Y, Entity.Width, Entity.Height);
            //LegendOf50.Instance.Graphics.SetColor(255, 255, 255, 255);

            return Task.CompletedTask;
        }
    }
}

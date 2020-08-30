using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Super50BrosGame
{
    // Entity Class
    public class Entity
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Dx { get; set; }
        public double Dy { get; set; }

        public int Width { get; }
        public int Height { get; }

        public string Texture { get; }
        public StateMachine StateMachine { get; set; }

        public Direction Direction { get; set; }

        public TileMap Map { get; }

        public GameLevel Level { get; set; }

        public Animation CurrentAnimation { get; set; }

        public Entity(double x, double y, int width, int height, string texture, StateMachine stateMachine, TileMap map, GameLevel level)
        {
            // position
            X = x;
            Y = y;

            // velocity
            Dx = 0;
            Dy = 0;

            // dimensions
            Width = width;
            Height = height;

            Texture = texture;
            StateMachine = stateMachine;

            Direction = Direction.Left;

            // reference to tilemap so we can check collisions
            Map = map;

            // reference to level for tests against other entities + objects
            Level = level;
        }

        public async Task ChangeState(string stateName, Dictionary<string, object> parameters = null)
        {
            await StateMachine.Change(stateName, parameters);
        }

        public async Task Update(TimeSpan dt)
        {
            await StateMachine.Update(dt);
        }

        public bool Collides(Entity entity)
        {
            return !(X > entity.X + entity.Width || entity.X > X + Width ||
                    Y > entity.Y + entity.Height || entity.Y > Y + Height);
        }

        public virtual void Render()
        {
            Super50Bros.Instance.Graphics.Draw(Super50Bros.Instance.Textures[Texture], Super50Bros.Instance.Frames[Texture][CurrentAnimation.GetCurrentFrame()],
                Math.Floor(X) + 8, Math.Floor(Y) + 10, Direction == Direction.Left, false, 8, 10);
        }
    }
}

using GameEngine;
using System;
using System.Threading.Tasks;

namespace Super50BrosGame
{
    // GameObject Class
    public class GameObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Texture { get; }
        public int Width { get; }
        public int Height { get; }
        public int Frame { get; }
        public bool? Solid { get; }
        public bool? Collidable { get; }
        public bool? Consumable { get; }
        public Action<GameObject> OnCollide { get; }
        public Action<Player, GameObject> OnConsume { get; }
        public bool? Hit { get; set; }

        public GameObject(string texture, double x, double y, int width, int height, int frame, bool? solid = null, bool? collidable = null, bool? consumable = null, Action<GameObject> onCollide = null, Action<Player, GameObject> onConsume = null, bool? hit = null)
        {
            Texture = texture;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Frame = frame;
            Solid = solid;
            Collidable = collidable;
            Consumable = consumable;
            OnCollide = onCollide;
            OnConsume = onConsume;
            Hit = hit;
        }

        public bool Collides(Entity target)
        {
            return !(target.X > X + Width || X > target.X + target.Width ||
                     target.Y > Y + Height || Y > target.Y + target.Height);
        }

        public virtual Task Update(TimeSpan dt)
        {
            return Task.CompletedTask;
        }

        public void Render(int camX, int camY)
        {
            var gameObjectBox = new Box((int)X, (int)Y, Width, Height);
            var screenBox = new Box(camX, camY, Super50Bros.Instance.VirtualWidth, Super50Bros.Instance.VirtualHeight);

            if (gameObjectBox.Intersect(screenBox))
            {
                Super50Bros.Instance.Graphics.Draw(Super50Bros.Instance.Textures[Texture], Super50Bros.Instance.Frames[Texture][Frame], X, Y);
            }
        }
    }
}

using FiftyBirdGame.States;
using GameEngine.Graphics;
using GameEngine.Input;
using System;
using System.Threading.Tasks;

namespace FiftyBirdGame
{
    // Bird Class

    // The Bird is what we control in the game via clicking or the space bar; whenever we press either,
    // the bird will flap and go up a little bit, where it will then be affected by gravity.If the bird hits
    // the ground or a pipe, the game is over.
    public class Bird
    {
        const double Gravity = 20;

        Image image;

        public double X { get; private set; }
        public double Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public double Dy { get; private set; }

        public Bird()
        {
            X = FiftyBird.Instance.VirtualWidth / 2 - 8;
            Y = FiftyBird.Instance.VirtualHeight / 2 - 8;

            Dy = 0;
        }

        public async Task Load()
        {
            image = await FiftyBird.Instance.Graphics.NewImage("bird.png");

            Width = image.Width;
            Height = image.Height;
        }

        // AABB collision that expects a pipe, which will have an X and Y and reference
        // global pipe width and height values.
        public bool Collides(Pipe pipe)
        {
            // the 2's are left and top offsets
            // the 4's are right and bottom offsets
            // both offsets are used to shrink the bounding box to give the player
            // a little bit of leeway with the collision
            if (((X + 2) + (Width - 4) >= pipe.X) && (X + 2 <= pipe.X + PlayState.PipeWidth))
            {
                if ((((Y + 2) + (Height - 4)) >= pipe.Y) && (Y + 2 <= pipe.Y + PlayState.PipeHeight))
                {
                    return true;
                }
            }

            return false;
        }

        public void Update(TimeSpan dt)
        {
            Dy += Gravity * dt.TotalSeconds;

            // burst of anti-gravity when space or left mouse are pressed
            if (FiftyBird.Instance.Keyboard.WasPressed(Key.Space) || FiftyBird.Instance.Mouse.WasPressed(MouseButton.Left))
            {
                Dy = -5;
                FiftyBird.Instance.Sounds["jump"].Play();
            }

            Y += Dy;
        }

        public void Render()
        {
            FiftyBird.Instance.Graphics.Draw(image, X, Y);
        }
    }
}

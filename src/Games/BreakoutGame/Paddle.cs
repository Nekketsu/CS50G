using GameEngine.Input;
using System;

namespace BreakoutGame
{
    // Paddle Class

    // Represents a paddle that can move left and right.Used in the main
    // program to deflect the ball toward the bricks; if the ball passes
    // the paddle, the player loses one heart.The Paddle can have a skin,
    // which the player gets to choose upon starting the game.
    public class Paddle
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public double Dx { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int Skin { get; }

        public int Size { get; private set; }

        // Our Paddle will initialize at the same spot every time, in the middle
        // of the world horizontally, toward the bottom.
        public Paddle(int skin)
        {
            // x is placed in the middle
            X = Breakout.Instance.VirtualWidth / 2 - 32;

            // y is placed a little above the bottom edge of the screen
            Y = Breakout.Instance.VirtualHeight - 32;

            // starts us off with no velocity
            Dx = 0;

            // starting dimensions
            Width = 64;
            Height = 16;

            // the skin only has the effect of changing our color, used to offset us
            // into the gPaddleSkins table later
            Skin = skin;

            // the variant is which of the four paddle sizes we currently are; 2
            // is the starting size, as the smallest is too tough to start with
            Size = 1;
        }

        public void Update(TimeSpan dt)
        {
            // keyboard input
            if (Breakout.Instance.Keyboard.IsDown(Key.Left))
            {
                Dx = -Breakout.Instance.PaddleSpeed;
            }
            else if (Breakout.Instance.Keyboard.IsDown(Key.Right))
            {
                Dx = Breakout.Instance.PaddleSpeed;
            }
            else
            {
                Dx = 0;
            }

            // math.max here ensures that we're the greater of 0 or the player's
            // current calculated Y position when pressing up so that we don't
            // go into the negatives; the movement calculation is simply our
            // previously-defined paddle speed scaled by dt
            if (Dx < 0)
            {
                X = Math.Max(0, X + Dx * dt.TotalSeconds);
            }
            // similar to before, this time we use math.min to ensure we don't
            // go any farther than the bottom of the screen minus the paddle's
            // height(or else it will go partially below, since position is
            // based on its top left corner)
            else
            {
                X = Math.Min(Breakout.Instance.VirtualWidth - Width, X + Dx * dt.TotalSeconds);
            }
        }

        // Render the paddle by drawing the main texture, passing in the quad
        // that corresponds to the proper skin and size.
        public void Render()
        {
            Breakout.Instance.Graphics.Draw(Breakout.Instance.Textures["main"], Breakout.Instance.Frames["paddles"][Size + 4 * Skin], X, Y);
        }
    }
}

using GameEngine.Graphics;
using System;

namespace PongGame
{
    // Paddle Class

    // Represents a paddle that can move up and down.Used in the main
    // program to deflect the ball back toward the opponent.
    public class Paddle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Heigth { get; set; }
        public double Dy { get; set; }

        // The `init` function on our class is called just once, when the object
        // is first created. Used to set up all variables in the class and get it
        // ready for use.

        // Our Paddle should take an X and a Y, for positioning, as well as a width
        // and height for its dimensions.

        // Note that `self` is a reference to *this* object, whichever object is
        // instantiated at the time this function is called. Different objects can
        // have their own x, y, width, and height values, thus serving as containers
        // for data. In this sense, they're very similar to structs in C.
        public Paddle(double x, double y, double width, double heigth)
        {
            X = x;
            Y = y;
            Width = width;
            Heigth = heigth;
            Dy = 0;
        }

        public void Update(TimeSpan dt)
        {
            // math.max here ensures that we're the greater of 0 or the player's
            // current calculated Y position when pressing up so that we don't
            // go into the negatives; the movement calculation is simply our
            // previously - defined paddle speed scaled by dt
            if (Dy < 0)
            {
                Y = Math.Max(0, Y + Dy * dt.TotalSeconds);
            }
            // similar to before, this time we use math.min to ensure we don't
            // go any farther than the bottom of the screen minus the paddle's
            // height(or else it will go partially below, since position is
            // based on its top left corner)
            else
            {
                Y = Math.Min(Pong.Instance.VirtualHeight - Heigth, Y + Dy * dt.TotalSeconds);
            }
        }
        // To be called by our main function in `love.draw`, ideally.Uses
        // LÖVE2D's `rectangle` function, which takes in a draw mode as the first
        // argument as well as the position and dimensions for the rectangle.To
        // change the color, one must call `love.graphics.setColor`. As of the
        // newest version of LÖVE2D, you can even draw rounded rectangles!
        public void Render()
        {
            Pong.Instance.Graphics.Rectangle(DrawMode.Fill, X, Y, Width, Heigth);
        }
    }
}

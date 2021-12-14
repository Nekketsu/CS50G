using GameEngine.Graphics;

namespace PongGame
{
    // Ball Class

    // Represents a ball which will bounce back and forth between paddles
    // and walls until it passes a left or right boundary of the screen,
    // scoring a point for the opponent.
    public class Ball
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double Dx { get; set; }
        public double Dy { get; set; }

        public Ball(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            Dx = 0;
            Dy = 0;
        }

        public bool Collides(Paddle paddle)
        {
            if ((X > paddle.X + paddle.Width) || (paddle.X > X + Width))
            {
                return false;
            }

            if ((Y > paddle.Y + paddle.Heigth) || (paddle.Y > Y + Height))
            {
                return false;
            }

            return true;
        }

        public void Reset()
        {
            X = Pong.Instance.VirtualWidth / 2 - 2;
            Y = Pong.Instance.VirtualHeight / 2 - 2;
            Dx = 0;
            Dy = 0;
        }

        public void Update(TimeSpan dt)
        {
            X += Dx * dt.TotalSeconds;
            Y += Dy * dt.TotalSeconds;
        }

        public void Render()
        {
            Pong.Instance.Graphics.Rectangle(DrawMode.Fill, X, Y, Width, Height);
        }
    }
}

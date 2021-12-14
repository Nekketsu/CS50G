namespace BreakoutGame
{
    // Ball Class

    // Represents a ball which will bounce back and forth between the sides
    // of the world space, the player's paddle, and the bricks laid out above
    // the paddle. The ball can have a skin, which is chosen at random, just
    // for visual variety.
    public class Ball
    {
        public double Y { get; set; }
        public double X { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public double Dx { get; set; }
        public double Dy { get; set; }

        public int Skin { get; }

        public Ball(int skin)
        {
            // simple positional and dimensional variables
            Width = 8;
            Height = 8;

            // these variables are for keeping track of our velocity on both the
            // X and Y axis, since the ball can move in two dimensions
            Dx = 0;
            Dy = 0;

            // this will effectively be the color of our ball, and we will index
            // our table of Quads relating to the global block texture using this
            Skin = skin;
        }

        // Expects an argument with a bounding box, be that a paddle or a brick,
        // and returns true if the bounding boxes of this and the argument overlap.
        public bool Collides(Paddle target)
        {
            // first, check to see if the left edge of either is farther to the right
            // than the right edge of the other
            if ((X > target.X + target.Width) || (target.X > X + Width))
            {
                return false;
            }

            // then check to see if the bottom edge of either is higher than the top
            // edge of the other
            if ((Y > target.Y + target.Height) || (target.Y > Y + Height))
            {
                return false;
            }

            // if the above aren't true, they're overlapping
            return true;
        }

        // Expects an argument with a bounding box, be that a paddle or a brick,
        // and returns true if the bounding boxes of this and the argument overlap.
        public bool Collides(Brick target)
        {
            // first, check to see if the left edge of either is farther to the right
            // than the right edge of the other
            if ((X > target.X + target.Width) || (target.X > X + Width))
            {
                return false;
            }

            // then check to see if the bottom edge of either is higher than the top
            // edge of the other
            if ((Y > target.Y + target.Height) || (target.Y > Y + Height))
            {
                return false;
            }

            // if the above aren't true, they're overlapping
            return true;
        }

        // Places the ball in the middle of the screen, with no movement.
        public void Reset()
        {
            X = Breakout.Instance.VirtualWidth / 2 - 2;
            Y = Breakout.Instance.VirtualHeight / 2 - 2;
            Dx = 0;
            Dy = 0;
        }

        public async Task Update(TimeSpan dt)
        {
            X += Dx * dt.TotalSeconds;
            Y += Dy * dt.TotalSeconds;

            // allow ball to bounce off walls
            if (X <= 0)
            {
                X = 0;
                Dx = -Dx;
                await Breakout.Instance.Sounds["wall-hit"].Play();
            }

            if (X >= Breakout.Instance.VirtualWidth - 8)
            {
                X = Breakout.Instance.VirtualWidth - 8;
                Dx = -Dx;
                await Breakout.Instance.Sounds["wall-hit"].Play();
            }

            if (Y <= 0)
            {
                Y = 0;
                Dy = -Dy;
                await Breakout.Instance.Sounds["wall-hit"].Play();
            }
        }

        public void Render()
        {
            // gTexture is our global texture for all blocks
            // gBallFrames is a table of quads mapping to each individual ball skin in the texture
            Breakout.Instance.Graphics.Draw(Breakout.Instance.Textures["main"], Breakout.Instance.Frames["balls"][Skin], X, Y);
        }
    }
}

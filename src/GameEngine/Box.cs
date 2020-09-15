namespace GameEngine
{
    public class Box
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public Box(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Intersect(Box box)
        {
            // If one box is on the left side of other
            if (X > box.X + box.Width || box.X > X + Width)
            {
                return false;
            }

            // If one box is above other
            if (Y > box.Y + box.Height || box.Y > Y + Height)
            {
                return false;
            }

            return true;
        }
    }
}

using GameEngine.Graphics;

namespace Match3Game
{
    // Tile Class

    // The individual tiles that make up our game board. Each Tile can have a
    // color and a variety, with the varietes adding extra points to the matches.
    public class Tile
    {
        public int GridX { get; set; }
        public int GridY { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public int Color { get; }
        public int Variety { get; }

        public Tile(int x, int y, int color, int variety)
        {
            // board positions
            GridX = x;
            GridY = y;

            // coordinate positions
            X = x * 32;
            Y = y * 32;

            // title appearance/points
            Color = color;
            Variety = variety;
        }

        public void Render(double x, double y)
        {
            // draw shadow
            Match3.Instance.Graphics.SetColor(34, 32, 52, 255);
            Match3.Instance.Graphics.Draw(Match3.Instance.Textures["main"], Match3.Instance.Frames["tiles"][Color][Variety], X + x + 2, Y + y + 2);
            // TODO: workaround to tint an image
            Match3.Instance.Graphics.Rectangle(DrawMode.Fill, (int)(X + x + 2), (int)(Y + y + 2), 32, 32, 4);

            // draw tile itself
            Match3.Instance.Graphics.SetColor(255, 255, 255, 255);
            Match3.Instance.Graphics.Draw(Match3.Instance.Textures["main"], Match3.Instance.Frames["tiles"][Color][Variety], X + x, Y + y);
        }
    }
}

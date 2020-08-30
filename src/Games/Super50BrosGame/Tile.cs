using System.Linq;

namespace Super50BrosGame
{
    // Tile Class
    public class Tile
    {
        public int X { get; }
        public int Y { get; }

        public int Width { get; }
        public int Height { get; }

        public int Id { get; }
        public bool? Topper { get; set; }
        public int Tileset { get; }
        public int Topperset { get; }

        public Tile(int x, int y, int id, bool? topper, int tileset, int topperset)
        {
            X = x;
            Y = y;

            Width = Super50Bros.Instance.TileSize;
            Height = Super50Bros.Instance.TileSize;

            Id = id;
            Tileset = tileset;
            Topper = topper;
            Topperset = topperset;
        }

        // Checks to see whether this ID is whitelisted as collidable in a global constants table.
        public bool Collidable()
        {
            return Super50Bros.Instance.CollidableTiles.Contains(Id);
        }

        public void Render()
        {
            Super50Bros.Instance.Graphics.Draw(Super50Bros.Instance.Textures["tiles"], Super50Bros.Instance.FrameSets["tilesets"][Tileset][Id],
                X * Super50Bros.Instance.TileSize, Y * Super50Bros.Instance.TileSize);

            // tile top layer for graphical variety
            if (Topper == true)
            {
                Super50Bros.Instance.Graphics.Draw(Super50Bros.Instance.Textures["toppers"], Super50Bros.Instance.FrameSets["toppersets"][Topperset][Id],
                    X * Super50Bros.Instance.TileSize, Y * Super50Bros.Instance.TileSize);
            }
        }
    }
}

using System;

namespace Super50BrosGame
{
    // TileMap Class
    public class TileMap
    {
        public int Width { get; }
        public int Height { get; }
        public Tile[,] Tiles { get; set; }

        public TileMap(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[height, width];
        }

        public TileMap(Tile[,] tiles)
        {
            Tiles = tiles;
            Width = tiles.GetLength(1);
            Height = tiles.GetLength(0);
        }

        // If our tiles were animated, this is potentially where we could iterate over all of them
        // and update either per-tile or per-map animations for appropriately flagged tiles!
        public void Update(TimeSpan dt)
        {

        }

        // Returns the x, y of a tile given an x, y of coordinates in the world space.
        public Tile PointToTile(double x, double y)
        {
            if (x < 0 || x > Width * Super50Bros.Instance.TileSize || y < 0 || y > Height * Super50Bros.Instance.TileSize)
            {
                return null;
            }

            return Tiles[(int)(y / Super50Bros.Instance.TileSize), (int)(x / Super50Bros.Instance.TileSize)];
        }

        public void Render()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    Tiles[y, x].Render();
                }
            }
        }
    }
}

using GameEngine.Graphics;
using System.Collections.Generic;

namespace Super50BrosGame
{
    // StartState Class

    // Helper functions for writing Match-3.
    public class Util
    {
        public Quad[] GenerateQuads(Image atlas, int tileWidth, int tileHeight)
        {
            var sheetWidth = atlas.Width / tileWidth;
            var sheetHeight = atlas.Height / tileHeight;

            var spritesheet = new List<Quad>();

            for (var y = 0; y < sheetHeight; y++)
            {
                for (var x = 0; x < sheetWidth; x++)
                {
                    spritesheet.Add(Super50Bros.Instance.Graphics.NewQuad(x * tileWidth, y * tileHeight, tileWidth, tileHeight, atlas.Width, atlas.Height));
                }
            }

            return spritesheet.ToArray();
        }

        // Divides quads we've generated via slicing our tile sheet into separate tile sets.
        public Quad[][] GenerateTileSets(Quad[] quads, int setsX, int setsY, int sizeX, int sizeY)
        {
            var tilesets = new List<Quad[]>();
            var sheetWidth = setsX * sizeX;
            var sheetHeight = setsY * sizeY;

            // for each tile set on the X and Y
            for (var tilesetY = 0; tilesetY < setsY; tilesetY++)
            {
                for (var tilesetX = 0; tilesetX < setsX; tilesetX++)
                {
                    // tileset table
                    var tileset = new List<Quad>();

                    for (var y = sizeY * tilesetY; y < sizeY * tilesetY + sizeY; y++)
                    {
                        for (var x = sizeX * tilesetX; x < sizeX * tilesetX + sizeX; x++)
                        {
                            tileset.Add(quads[sheetWidth * y + x]);
                        }
                    }

                    tilesets.Add(tileset.ToArray());
                }
            }

            return tilesets.ToArray();
        }
    }
}

using GameEngine.Graphics;

namespace LegendOf50Game
{
    // StartState Class

    // Helper functions for writing Match-3.
    public class Utils
    {
        // Given an "atlas" (a texture with multiple sprites), as well as a
        // width and a height for the tiles therein, split the texture into
        // all of the quads by simply dividing it evenly.
        public Quad[] GenerateQuads(Image atlas, int tileWidth, int tileHeight)
        {
            var sheetWidth = atlas.Width / tileWidth;
            var sheetHeight = atlas.Height / tileHeight;

            var spritesheet = new List<Quad>();

            for (var y = 0; y < sheetHeight; y++)
            {
                for (var x = 0; x < sheetWidth; x++)
                {
                    spritesheet.Add(LegendOf50.Instance.Graphics.NewQuad(x * tileWidth, y * tileHeight, tileWidth, tileHeight, atlas.Width, atlas.Height));
                }
            }

            return spritesheet.ToArray();
        }
    }
}

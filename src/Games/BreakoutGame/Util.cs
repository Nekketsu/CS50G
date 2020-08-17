using GameEngine.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace BreakoutGame
{
    // StartState Class

    //Helper functions for writing games.
    public class Util
    {
        // Given an "atlas" (a texture with multiple sprites), as well as a
        // width and a height for the tiles therein, split the texture into
        // all of the quads by simply dividing it evenly.
        public Quad[] GenerateQuads(Image atlas, int tileWidth, int tileHeight)
        {
            var sheetWidth = atlas.Width / tileWidth;
            var sheetHeight = atlas.Height / tileHeight;

            var spriteSheet = new List<Quad>();

            for (var y = 0; y < sheetHeight; y++)
            {
                for (var x = 0; x < sheetWidth; x++)
                {
                    spriteSheet.Add(Breakout.Instance.Graphics.NewQuad(x * tileWidth, y * tileHeight, tileWidth, tileHeight, atlas.Width, atlas.Height));
                }
            }

            return spriteSheet.ToArray();
        }

        // This function is specifically made to piece out the bricks from the
        // sprite sheet.Since the sprite sheet has non-uniform sprites within,
        // we have to return a subset of GenerateQuads.
        public Quad[] GenerateQuadsBricks(Image atlas)
        {
            return GenerateQuads(atlas, 32, 16).Take(20).ToArray();
        }

        // This function is specifically made to piece out the paddles from the
        // sprite sheet.For this, we have to piece out the paddles a little more
        // manually, since they are all different sizes.
        public Quad[] GenerateQuadsPaddles(Image atlas)
        {
            var x = 0;
            var y = 64;

            var quads = new List<Quad>();

            for (var i = 0; i < 4; i++)
            {
                // smallest
                quads.Add(Breakout.Instance.Graphics.NewQuad(x, y, 32, 16, atlas.Width, atlas.Height));
                // medium
                quads.Add(Breakout.Instance.Graphics.NewQuad(x + 32, y, 64, 16, atlas.Width, atlas.Height));
                // large
                quads.Add(Breakout.Instance.Graphics.NewQuad(x + 96, y, 96, 16, atlas.Width, atlas.Height));
                // huge
                quads.Add(Breakout.Instance.Graphics.NewQuad(x, y + 16, 128, 16, atlas.Width, atlas.Height));

                // prepare X and Y for the next set of paddles
                x = 0;
                y += 32;
            }

            return quads.ToArray();
        }

        //This function is specifically made to piece out the balls from the
        //sprite sheet.For this, we have to piece out the balls a little more
        //manually, since they are in an awkward part of the sheet and small.
        public Quad[] GenerateQuadsBalls(Image atlas)
        {
            var x = 96;
            var y = 48;

            var quads = new List<Quad>();

            for (var i = 0; i < 4; i++)
            {
                quads.Add(Breakout.Instance.Graphics.NewQuad(x, y, 8, 8, atlas.Width, atlas.Height));
                x += 8;
            }

            x = 96;
            y = 56;

            for (var i = 0; i < 3; i++)
            {
                quads.Add(Breakout.Instance.Graphics.NewQuad(x, y, 8, 8, atlas.Width, atlas.Height));
                x += 8;
            }

            return quads.ToArray();
        }
    }
}

using GameEngine.Graphics;

namespace Match3Game
{
    // StartState Class

    // Helper functions for writing Match-3.
    public class Util
    {
        // Given an "atlas" (a texture with multiple sprites), generate all of the
        // quads for the different tiles therein, divided into tables for each set
        // of tiles, since each color has 6 varieties.
        public Quad[][] GenerateTileQuads(Image atlas)
        {
            var tiles = new List<List<Quad>>();

            var x = 0;
            var y = 0;

            // 9 rows of tiles
            for (var row = 0; row < 9; row++)
            {
                // two sets of 6 cols, different tile varietes
                for (var i = 0; i < 2; i++)
                {
                    var tileRow = new List<Quad>();
                    tiles.Add(tileRow);

                    for (var col = 0; col < 6; col++)
                    {
                        tileRow.Add(Match3.Instance.Graphics.NewQuad(x, y, 32, 32, atlas.Width, atlas.Height));
                        x += 32;
                    }
                }

                y += 32;
                x = 0;
            }

            return tiles.Select(tileRow => tileRow.ToArray()).ToArray();
        }

        // Recursive table printing function.
        // https://coronalabs.com/blog/2014/09/02/tutorial-printing-table-contents/
    }
}

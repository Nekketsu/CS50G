using System.Collections.Generic;
using System.Linq;

namespace Match3Game
{
    public class Board
    {
        public int X { get; }
        public int Y { get; }

        public Tile[][] Matches { get; private set; }
        public Tile[,] Tiles { get; private set; }

        public Board(int x, int y)
        {
            X = x;
            Y = y;

            Tiles = new Tile[8, 8];
            InitializeTiles();
        }

        private void InitializeTiles()
        {
            for (var tileY = 0; tileY < 8; tileY++)
            {
                // empty table taht will serve as a new row

                for (var tileX = 0; tileX < 8; tileX++)
                {
                    // create a new tile at X,Y with a random color and variety
                    Tiles[tileY, tileX] = new Tile(tileX, tileY, Match3.Instance.Random.Next(18), Match3.Instance.Random.Next(6));
                }
            }

            while (CalculateMatches().Any())
            {
                // recursively initialize if matches were returned so we always have
                // a matchless board on start
                InitializeTiles();
            }
        }

        // Goes left to right, top to bottom in the board, calculating matches by counting consecutive
        // tiles of the same color. Doesn't need to check the last tile in every row or column if the 
        // last two haven't been a match.
        public Tile[][] CalculateMatches()
        {
            var matches = new List<Tile[]>();

            // how many of the same color blocks in a row we've found
            int matchNum;

            // horizontal matches first
            for (var y = 0; y < 8; y++)
            {
                var colorToMatch = Tiles[y, 0].Color;

                matchNum = 1;

                // every horizontal tile
                for (var x = 1; x < 8; x++)
                {
                    // if this is the same color as the one we're trying to match...
                    if (Tiles[y, x].Color == colorToMatch)
                    {
                        matchNum++;
                    }
                    else
                    {
                        // set this as the new color we want to watch for
                        colorToMatch = Tiles[y, x].Color;

                        // if we have a match of 3 or more up to now, add it to our matches table
                        if (matchNum >= 3)
                        {
                            var match = new List<Tile>();
                            // go backwards from here by matchNum
                            for (var x2 = x - 1; x2 >= x - matchNum; x2--)
                            {
                                // add each tile to the match that's in that match
                                match.Add(Tiles[y, x2]);
                            }

                            // add this match to our total matches table
                            matches.Add(match.ToArray());
                        }

                        matchNum = 1;

                        // don't need to check last two if they won't be in a match
                        if (x >= 8 - 1 - 1)
                        {
                            break;
                        }
                    }
                }

                // account for the last row ending with a match
                if (matchNum >= 3)
                {
                    var match = new List<Tile>();

                    // go backwards from end of last row by matchNum
                    for (var x = 8 - 1; x >= 8 - matchNum; x--)
                    {
                        match.Add(Tiles[y, x]);
                    }

                    matches.Add(match.ToArray());
                }
            }

            // vertical matches
            for (var x = 0; x < 8; x++)
            {
                var colorToMatch = Tiles[0, x].Color;

                matchNum = 1;

                // every vertical tile
                for (var y = 1; y < 8; y++)
                {
                    if (Tiles[y, x].Color == colorToMatch)
                    {
                        matchNum++;
                    }
                    else
                    {
                        colorToMatch = Tiles[y, x].Color;

                        if (matchNum >= 3)
                        {
                            var match = new List<Tile>();

                            for (var y2 = y - 1; y2 >= y - matchNum; y2--)
                            {
                                match.Add(Tiles[y2, x]);
                            }

                            matches.Add(match.ToArray());
                        }

                        matchNum = 1;

                        // don't need to check last two if they won't be in a match
                        if (y >= 8 - 1 - 1)
                        {
                            break;
                        }
                    }
                }

                // account for the last column ending with a match
                if (matchNum >= 3)
                {
                    var match = new List<Tile>();

                    // go backwards from end of last row by matchNum
                    for (var y = 8 - 1; y >= 8 - matchNum; y--)
                    {
                        match.Add(Tiles[y, x]);
                    }

                    matches.Add(match.ToArray());
                }
            }

            // store matches for later reference
            Matches = matches.ToArray();

            // return matches table if > 0, else just return false
            return Matches;
        }

        // Remove the matches from the Board by just setting the Tile slots within
        // them to nil, then setting self.matches to nil.
        public void RemoveMatches()
        {
            foreach (var match in Matches)
            {
                foreach (var tile in match)
                {
                    Tiles[tile.GridY, tile.GridX] = null;
                }
            }

            Matches = null;
        }

        // Shifts down all of the tiles that now have spaces below them, then returns a table that
        // contains tweening information for these new tiles.
        public Dictionary<Tile, int> GetFallingTiles()
        {
            // tween table, with tiles as keys and their x and y as the to values
            var tweens = new Dictionary<Tile, int>();

            // for each column, go up tile by tile till we hit a space

            for (var x = 0; x < 8; x++)
            {
                var space = false;
                var spaceY = -1;

                var y = 8 - 1;
                while (y >= 0)
                {
                    // if our last tile was a space...
                    var tile = Tiles[y, x];

                    if (space)
                    {
                        // if the current tile is *not* a space, bring this down to the lowest space
                        if (tile != null)
                        {
                            // put the tile in the correct spot in the board and fix its grid positions
                            Tiles[spaceY, x] = tile;
                            tile.GridY = spaceY;

                            // set its prior position to null
                            Tiles[y, x] = null;

                            // tween the Y position to 32 x its grid position
                            tweens[tile] = tile.GridY * 32;

                            // set Y to spaceY so we start back from here again
                            space = false;
                            y = spaceY;

                            // set this back to -1 so we know we don't have an active space
                            spaceY = -1;
                        }
                    }
                    else if (tile == null)
                    {
                        space = true;

                        // if we haven't assigned a space yet, set this to it
                        if (spaceY == -1)
                        {
                            spaceY = y;
                        }
                    }

                    y--;
                }
            }

            // create replacement tiles at the top of the screen
            for (var x = 0; x < 8; x++)
            {
                for (var y = 8 - 1; y >= 0; y--)
                {
                    var tile = Tiles[y, x];

                    // if the tile is nil, we need to add a new one
                    if (tile == null)
                    {
                        // new tile with random color and variety
                        var newTile = new Tile(x, y, Match3.Instance.Random.Next(18), Match3.Instance.Random.Next(6));
                        newTile.Y = -32;
                        Tiles[y, x] = newTile;

                        // create a new tween to return for this tile to fall down
                        tweens[newTile] = newTile.GridY * 32;
                    }
                }
            }

            return tweens;
        }

        public void Render()
        {
            foreach (var tile in Tiles)
            {
                tile.Render(X, Y);
            }
        }
    }
}

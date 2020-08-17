using System;
using System.Collections.Generic;

namespace BreakoutGame
{
    // LevelMaker Class

    // Creates randomized levels for our Breakout game. Returns a table of
    // bricks that the game can render, based on the current level we're at
    // in the game.
    public class LevelMaker
    {
        // Creates a table of Bricks to be returned to the main game, with different
        // possible ways of randomizing rows and columns of bricks. Calculates the
        // brick colors and tiers to choose based on the level passed in.
        public Brick[] CreateMap(int level)
        {
            var bricks = new List<Brick>();

            // randomly choose the number of rows
            var numRows = Breakout.Instance.Random.Next(1, 5 + 1);

            // randomly choose the number of columns, ensuring odd
            var numCols = Breakout.Instance.Random.Next(7, 13 + 1);
            numCols = numCols % 2 == 0 ? numCols + 1 : numCols;

            // highest possible spawned brick color in this level; ensure we
            // don't go above 3
            var highestTier = Math.Min(3, level / 5);

            // highest color of the highest tier, no higher than 5
            var highestColor = Math.Min(5, level % 5 + 3);

            // lay out bricks such that they touch each other and fill the space
            for (var y = 0; y < numRows; y++)
            {
                // whether we want to enable skipping for this row
                var skipPattern = Breakout.Instance.Random.Next(2) == 1;

                // whether we want to enable alternating colors for this row
                var alternatePattern = Breakout.Instance.Random.Next(2) == 1;

                // choose two colors to alternate between
                var alternateColor1 = Breakout.Instance.Random.Next(highestColor);
                var alternateColor2 = Breakout.Instance.Random.Next(highestColor);
                var alternateTier1 = Breakout.Instance.Random.Next(highestTier + 1);
                var alternateTier2 = Breakout.Instance.Random.Next(highestTier + 1);

                // used only when we want to skip a block, for skip pattern
                var skipFlag = Breakout.Instance.Random.Next(2) == 1;

                // used only when we want to alternate a block, for alternate pattern
                var alternateFlag = Breakout.Instance.Random.Next(2) == 1;

                // solid color we'll use if we're not skipping or alternating
                var solidColor = Breakout.Instance.Random.Next(highestColor);
                var solidTier = Breakout.Instance.Random.Next(highestTier + 1);

                for (var x = 0; x < numCols; x++)
                {
                    // if skipping is turned on and we're on a skip iteration...
                    if (skipPattern && skipFlag)
                    {
                        skipFlag = !skipFlag;

                        continue;
                    }
                    else
                    {
                        // flip the flag to true on an iteration we don't use it
                        skipFlag = !skipFlag;
                    }

                    var b = new Brick(
                        x                      // x-coordinate
                        * 32                   // multiply by 32, the brick width
                        + 8                    // the screen should have 8 pixels of padding; we can fit 13 cols + 16 pixels total
                        + (13 - numCols) * 16, // left-side padding for when there are fewer than 13 columns

                        (y + 1) * 16           // just use (y + 1) * 16, since we need top padding anyway
                    );

                    // if we're alternating, figure out which color/tier we're on
                    if (alternatePattern && alternateFlag)
                    {
                        b.Color = alternateColor1;
                        b.Tier = alternateTier1;
                        alternateFlag = !alternateFlag;
                    }
                    else
                    {
                        b.Color = alternateColor2;
                        b.Tier = alternateTier2;
                        alternateFlag = !alternateFlag;
                    }

                    // if not alternating and we made it here, use the solid color/tier
                    if (!alternatePattern)
                    {
                        b.Color = solidColor;
                        b.Tier = solidTier;
                    }

                    bricks.Add(b);
                }
            }

            // in the event we didn't generate any bricks, try again
            if (bricks.Count == 0)
            {
                return CreateMap(level);
            }
            else
            {
                return bricks.ToArray();
            }
        }
    }
}

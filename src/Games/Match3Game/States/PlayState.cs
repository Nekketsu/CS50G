using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Match3Game.States
{
    // PlayState Class

    // State in which we can actually play, moving around a grid cursor that
    // can swap two tiles; when two tiles make a legal swap (a swap that results
    // in a valid match), perform the swap and destroy all matched tiles, adding
    // their values to the player's point score. The player can continue playing
    // until they exceed the number of points needed to get to the next level
    // or until the time runs out, at which point they are brought back to the
    // main menu or the score entry menu if they made the top 10.
    public class PlayState : State
    {
        // start our transition alpha at full, so we fade in
        int transitionAlpha;

        // position in the grid which we're highlighting
        int boardHighlightX;
        int boardHighlightY;

        // timer used to switch the highlight rect's color
        bool rectHighlighted;

        // flag to show whether we're able to process input (not swapping or clearing)
        bool canInput;

        // tile we're currently highlighting (preparing to swap)
        Tile highlightedTile;

        int score;
        int timer;

        int level;
        Board board;
        int scoreGoal;

        public override Task Enter(Dictionary<string, object> parameters)
        {
            // grab level # from the params we're passed
            level = (int)parameters["level"];

            // spawn a board and place it toward the right
            board = parameters.ContainsKey("board") ? (Board)parameters["board"] : new Board(Match3.Instance.VirtualWidth - 272, 16);

            // grab score from params if it was passed
            score = parameters.ContainsKey("score") ? (int)parameters["score"] : 0;

            // score we have to reach to get to the next level
            scoreGoal = (int)(level * 1.25 * 1000);


            // start our transition alpha at full, so we fade in
            transitionAlpha = 255;

            // position in the grid which we're highlighting
            boardHighlightX = 0;
            boardHighlightY = 1;

            // timer used to switch the highlight rect's color
            rectHighlighted = false;

            // flag to show whether we're able to process input (not swapping or clearing)
            canInput = true;

            // tile we're currently highlighting (preparing to swap)
            highlightedTile = null;

            score = 0;
            timer = 60;

            // set our Timer class to turn cursor highlight on and off
            Match3.Instance.Timer.Every(TimeSpan.FromSeconds(0.5), () => rectHighlighted = !rectHighlighted);

            // subtract 1 from timer every second
            Match3.Instance.Timer.Every(TimeSpan.FromSeconds(1), async () =>
            {
                timer--;

                // play warning sound on timer if we get low
                if (timer <= 5)
                {
                    await Match3.Instance.Sounds["clock"].Play();
                }
            });

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            if (Match3.Instance.Keyboard.WasPressed(Key.Escape))
            {
                // Quit
            }

            // go back to start if time runs out
            if (timer <= 0)
            {
                // clear timers from prior PlayStates
                Match3.Instance.Timer.Clear();

                await Match3.Instance.Sounds["game-over"].Play();

                await Match3.Instance.StateMachine.Change("game-over", new Dictionary<string, object>
                {
                    ["score"] = score
                });
            }

            // go to next level if we surpass score goal
            if (score >= scoreGoal)
            {
                // clear timers from prior PlayStates
                // always clear before you change state, else next state's timers
                // will also clear!
                Match3.Instance.Timer.Clear();

                await Match3.Instance.Sounds["next-level"].Play();

                // change to begin game state with new level (incremented)
                await Match3.Instance.StateMachine.Change("begin-game", new Dictionary<string, object>
                {
                    ["level"] = level + 1,
                    ["score"] = score
                });
            }

            if (canInput)
            {
                // move cursor around based on bounds of grid, playing sounds
                if (Match3.Instance.Keyboard.WasPressed(Key.Up))
                {
                    boardHighlightY = Math.Max(0, boardHighlightY - 1);
                    await Match3.Instance.Sounds["select"].Play();
                }
                else if (Match3.Instance.Keyboard.WasPressed(Key.Down))
                {
                    boardHighlightY = Math.Min(7, boardHighlightY + 1);
                    await Match3.Instance.Sounds["select"].Play();
                }
                else if (Match3.Instance.Keyboard.WasPressed(Key.Left))
                {
                    boardHighlightX = Math.Max(0, boardHighlightX - 1);
                    await Match3.Instance.Sounds["select"].Play();
                }
                else if (Match3.Instance.Keyboard.WasPressed(Key.Right))
                {
                    boardHighlightX = Math.Min(7, boardHighlightX + 1);
                    await Match3.Instance.Sounds["select"].Play();
                }

                // if we've pressed enter, to select or deselect a tile...
                if (Match3.Instance.Keyboard.WasPressed(Key.Space) || Match3.Instance.Keyboard.WasPressed(Key.Enter) || Match3.Instance.Keyboard.WasPressed(Key.Return))
                {
                    // if same tile as currently highlighted, deselect
                    var x = boardHighlightX;
                    var y = boardHighlightY;

                    // if nothing is highlighted, highlight current tile
                    if (highlightedTile == null)
                    {
                        highlightedTile = board.Tiles[y, x];
                    }

                    // if we select the position already highlighted, remove highlight
                    else if (highlightedTile == board.Tiles[y, x])
                    {
                        highlightedTile = null;
                    }

                    // if the difference between X and Y combined of this highlighted tile
                    // vs the previous is not equal to 1, also remove highlight
                    else if (Math.Abs(highlightedTile.GridX - x) + Math.Abs(highlightedTile.GridY - y) > 1)
                    {
                        await Match3.Instance.Sounds["error"].Play();
                        highlightedTile = null;
                    }
                    else
                    {
                        // swap grid positions of tiles
                        var tempX = highlightedTile.GridX;
                        var tempY = highlightedTile.GridY;

                        var newTile = board.Tiles[y, x];

                        highlightedTile.GridX = newTile.GridX;
                        highlightedTile.GridY = newTile.GridY;
                        newTile.GridX = tempX;
                        newTile.GridY = tempY;

                        // swap tiles in the tiles table
                        board.Tiles[highlightedTile.GridY, highlightedTile.GridX] = highlightedTile;

                        board.Tiles[newTile.GridY, newTile.GridX] = newTile;

                        var newTileX = newTile.X;
                        var newTileY = newTile.Y;
                        var highlightedTileX = highlightedTile.X;
                        var highlightedTileY = highlightedTile.Y;
                        // tween coordinates between the two so they swap
                        Match3.Instance.Timer.Tween(TimeSpan.FromSeconds(0.1), 0, 1, value =>
                        {
                            newTile.X = value * highlightedTileX + (1 - value) * newTileX;
                            newTile.Y = value * highlightedTileY + (1 - value) * newTileY;
                            highlightedTile.X = value * newTileX + (1 - value) * highlightedTileX;
                            highlightedTile.Y = value * newTileY + (1 - value) * highlightedTileY;
                        }, async () =>
                        {
                            // once the swap is finished, we can tween falling blocks as needed
                            await CalculateMatches();
                        });
                    }
                }
            }

            // Match3.Instance.Timer.Update(dt);
        }

        // Calculates whether any matches were found on the board and tweens the needed
        // tiles to their new destinations if so. Also removes tiles from the board that
        // have matched and replaces them with new randomized tiles, deferring most of this
        // to the Board class.
        private async Task CalculateMatches()
        {
            highlightedTile = null;

            // if we have any matches, remove them and tween the falling blocks that result
            var matches = board.CalculateMatches();

            if (matches.Any())
            {
                await Match3.Instance.Sounds["match"].Stop();
                await Match3.Instance.Sounds["match"].Play();

                // add score for each match
                foreach (var match in matches)
                {
                    score += match.Length * 50;
                }

                // remove any tiles that matched from the board, making empty spaces
                board.RemoveMatches();

                // gets a table with tween values for tiles that should now fall
                var tilesToFall = board.GetFallingTiles();

                var tiles = tilesToFall.Select(tile => new { Tile = tile.Key, InitialY = tile.Key.Y, FinalY = tile.Value }).ToArray();
                Match3.Instance.Timer.Tween(TimeSpan.FromSeconds(0.25), 0, 1, value =>
                {
                    foreach (var tile in tiles)
                    {
                        tile.Tile.Y = tile.InitialY * (1 - value) + tile.FinalY * value;
                    }
                }, async () =>
                {
                    // recursively call function in case new matches have been created
                    // as a result of falling blocks once new blocks have finished falling
                    await CalculateMatches();
                });
            }

            // if no matches, we can continue playing
            else
            {
                canInput = true;
            }
        }

        public override Task Render()
        {
            // render board of tiles
            board.Render();

            // render highlighted tile if it exists
            if (highlightedTile != null)
            {
                // multiply so drawing white rect makes it brighter
                // Match3.Instance.Graphics.SetBlendMode("add");

                Match3.Instance.Graphics.SetColor(255, 255, 255, 96);
                Match3.Instance.Graphics.Rectangle(DrawMode.Fill, highlightedTile.GridX * 32 + (Match3.Instance.VirtualWidth - 272), highlightedTile.GridY * 32 + 16, 32, 32, 4);

                // back to alpha
                //Match3.Instance.Graphics.SetBlendMode("alpha");
            }

            // render highlight rect color based on timer
            if (rectHighlighted)
            {
                Match3.Instance.Graphics.SetColor(217, 87, 99, 255);
            }
            else
            {
                Match3.Instance.Graphics.SetColor(172, 50, 50, 255);
            }

            // draw actual cursor rect
            Match3.Instance.Graphics.SetLineWidth(4);
            Match3.Instance.Graphics.Rectangle(DrawMode.Line, boardHighlightX * 32 + (Match3.Instance.VirtualWidth - 272), boardHighlightY * 32 + 16, 32, 32, 4);

            // GUI text
            Match3.Instance.Graphics.SetColor(56, 56, 56, 234);
            Match3.Instance.Graphics.Rectangle(DrawMode.Fill, 16, 16, 186, 116, 4);

            Match3.Instance.Graphics.SetColor(99, 155, 255, 255);
            Match3.Instance.Graphics.SetFont(Match3.Instance.Fonts["medium"]);
            Match3.Instance.Graphics.Print($"Level: {level}", 20, 24, 182, Alignment.Center);
            Match3.Instance.Graphics.Print($"Score: {score}", 20, 52, 182, Alignment.Center);
            Match3.Instance.Graphics.Print($"Goal : {scoreGoal}", 20, 80, 182, Alignment.Center);
            Match3.Instance.Graphics.Print($"Timer: {timer}", 20, 108, 182, Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

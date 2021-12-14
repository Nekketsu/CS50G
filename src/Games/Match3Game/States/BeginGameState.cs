using GameEngine.Graphics;
using GameEngine.States;

namespace Match3Game.States
{
    //-- BeginGameState Class --

    // Represents the state the game is in right before we start playing;
    // should fade in, display a drop-down "Level X" message, then transition
    // to the PlayState, where we can finally use player input.
    public class BeginGameState : State
    {
        int transitionAlpha;
        Board board;
        int levelLabelY;

        int level;

        public override Task Enter(Dictionary<string, object> parameters)
        {
            // start our transition alpha at full, so we fade in
            transitionAlpha = 255;

            // spawn a board and place it toward the right
            board = new Board(Match3.Instance.VirtualWidth - 272, 16);

            // start our level # label off-screen
            levelLabelY = -64;


            // grab level # from the def we're passed
            level = (int)parameters["level"];

            // animate our white screen fade-in, then animate a drop-down with
            // the level text

            // first, over a period of 1 second, transition our alpha to 0
            Match3.Instance.Timer.Tween(TimeSpan.FromSeconds(1), transitionAlpha, 0, value => transitionAlpha = (int)value, () =>
            {
                // once that's finished, start a transition of our text label to
                // the center of the screen over 0.25 seconds
                Match3.Instance.Timer.Tween(TimeSpan.FromSeconds(0.25), levelLabelY, Match3.Instance.VirtualHeight / 2 - 8, value => levelLabelY = (int)value, () =>
                {
                    // after that, pause for one second with Timer.after
                    Match3.Instance.Timer.After(TimeSpan.FromSeconds(1), () =>
                    {
                        // then, animate the label going down past the bottom edge
                        Match3.Instance.Timer.Tween(TimeSpan.FromSeconds(0.25), Match3.Instance.VirtualHeight / 2 - 8, Match3.Instance.VirtualHeight + 30, value => levelLabelY = (int)value, async () =>
                        {
                            Match3.Instance.Timer.Clear();
                            // once that's complete, we're ready to play!
                            await Match3.Instance.StateMachine.Change("play", new Dictionary<string, object>
                            {
                                ["level"] = level,
                                ["board"] = board
                            });
                        });
                    });
                    return Task.CompletedTask;
                });
                return Task.CompletedTask;
            });

            return Task.CompletedTask;
        }

        //public override Task Update(TimeSpan dt)
        //{
        //    Timer.Update(dt);
        //}

        public override Task Render()
        {
            // render board of tiles
            board.Render();

            // render Level # label and background rect
            Match3.Instance.Graphics.SetColor(95, 205, 228, 200);
            Match3.Instance.Graphics.Rectangle(DrawMode.Fill, 0, levelLabelY - 8, Match3.Instance.VirtualWidth, 48);
            Match3.Instance.Graphics.SetColor(255, 255, 255, 255);
            Match3.Instance.Graphics.SetFont(Match3.Instance.Fonts["large"]);
            Match3.Instance.Graphics.Print($"Level {level}", 0, levelLabelY, Match3.Instance.VirtualWidth, Alignment.Center);

            // our transition foreground rectangle
            Match3.Instance.Graphics.SetColor(255, 255, 255, transitionAlpha);
            Match3.Instance.Graphics.Rectangle(DrawMode.Fill, 0, 0, Match3.Instance.VirtualWidth, Match3.Instance.VirtualHeight);

            return Task.CompletedTask;
        }
    }
}

using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;

namespace BreakoutGame.States
{
    // PlayState Class

    // Represents the screen where we can view all high scores previously recorded.
    public class HighScoreState : State
    {
        ScoreData[] highScores;

        public override Task Enter(Dictionary<string, object> parameters)
        {
            highScores = (ScoreData[])parameters["highScores"];

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            // return to the start screen if we press escape
            if (Breakout.Instance.Keyboard.WasPressed(Key.Enter) || Breakout.Instance.Keyboard.WasPressed(Key.Return))
            {
                await Breakout.Instance.Sounds["wall-hit"].Play();

                await Breakout.Instance.StateMachine.Change("start", new Dictionary<string, object>
                {
                    ["highScores"] = highScores
                });
            }
        }

        public override Task Render()
        {
            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["large"]);
            Breakout.Instance.Graphics.Print($"High Scores", 0, 20, Breakout.Instance.VirtualWidth, Alignment.Center);

            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["medium"]);

            // iterate over all high score indices in our high scores table
            for (var i = 0; i < 10; i++)
            {
                var highScore = highScores[i];

                // score number (1-10)
                Breakout.Instance.Graphics.Print($"{i + 1}.", Breakout.Instance.VirtualWidth / 4, 60 + i * 13, 50, Alignment.Left);

                // score name
                Breakout.Instance.Graphics.Print(highScore.Name, Breakout.Instance.VirtualWidth / 4 + 38, 60 + i * 13, 50, Alignment.Right);

                // score itself
                Breakout.Instance.Graphics.Print($"{highScore.Score}", Breakout.Instance.VirtualWidth / 2, 60 + i * 13, 100, Alignment.Right);
            }

            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["small"]);
            Breakout.Instance.Graphics.Print("Press Enter to return to the main menu!", 0, Breakout.Instance.VirtualHeight - 18, Breakout.Instance.VirtualWidth, Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

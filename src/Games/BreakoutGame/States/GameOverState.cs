using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreakoutGame.States
{
    public class GameOverState : State
    {
        int score;
        ScoreData[] highScores;

        public override Task Enter(Dictionary<string, object> parameters)
        {
            score = (int)parameters["score"];
            highScores = (ScoreData[])parameters["highScores"];

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            if (Breakout.Instance.Keyboard.WasPressed(Key.Enter) || Breakout.Instance.Keyboard.WasPressed(Key.Return))
            {
                // see if score is higher than any in the high scores table
                var highScore = false;

                // keep track of what high score ours overwrites, if any
                var highScoreIndex = 10;

                for (var i = 10 - 1; i >= 0; i--)
                {
                    var score = highScores[i].Score;
                    if (this.score > score)
                    {
                        highScoreIndex = i;
                        highScore = true;
                    }
                }

                if (highScore)
                {
                    await Breakout.Instance.Sounds["high-score"].Play();
                    await Breakout.Instance.StateMachine.Change("enter-high-score", new Dictionary<string, object>
                    {
                        ["highScores"] = highScores,
                        ["score"] = score,
                        ["scoreIndex"] = highScoreIndex
                    });
                }
                else
                {
                    await Breakout.Instance.StateMachine.Change("start", new Dictionary<string, object>
                    {
                        ["highScores"] = highScores
                    });
                }
            }

            if (Breakout.Instance.Keyboard.WasPressed(Key.Escape))
            {
                // Quit
            }
        }

        public override Task Render()
        {
            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["large"]);
            Breakout.Instance.Graphics.Print("GAME OVER", 0, Breakout.Instance.VirtualHeight / 3, Breakout.Instance.VirtualWidth, Alignment.Center);
            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["medium"]);
            Breakout.Instance.Graphics.Print($"Final Score: {score}", 0, Breakout.Instance.VirtualHeight / 2, Breakout.Instance.VirtualWidth, Alignment.Center);
            Breakout.Instance.Graphics.Print($"Press Enter!", 0, Breakout.Instance.VirtualHeight - Breakout.Instance.VirtualHeight / 4, Breakout.Instance.VirtualWidth, Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

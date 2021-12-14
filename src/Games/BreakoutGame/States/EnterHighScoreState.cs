using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using System.Text;

namespace BreakoutGame.States
{
    //EnterHighScoreState Class

    //Screen that allows us to input a new high score in the form of three characters, arcade-style.
    public class EnterHighScoreState : State
    {
        // individual chars of our string
        char[] chars = { (char)65, (char)65, (char)65 };

        // char we're currently changing
        int highlightedChar = 0;

        ScoreData[] highScores;
        int score;
        int scoreIndex;

        public override Task Enter(Dictionary<string, object> parameters)
        {
            highScores = (ScoreData[])parameters["highScores"];
            score = (int)parameters["score"];
            scoreIndex = (int)parameters["scoreIndex"];

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            if (Breakout.Instance.Keyboard.WasPressed(Key.Enter) || Breakout.Instance.Keyboard.WasPressed(Key.Return))
            {
                // update scores table
                var name = new string(chars);

                // go backwards through high scores table till this score, shifting scores
                for (var i = 10 - 1; i > scoreIndex; i--)
                {
                    highScores[i] = new ScoreData(highScores[i - 1].Name, highScores[i - 1].Score);
                }

                highScores[scoreIndex] = new ScoreData(name, score);

                // write socres to file
                var scoresStr = new StringBuilder();

                foreach (var highScore in highScores)
                {
                    scoresStr.AppendLine(highScore.Name);
                    scoresStr.AppendLine($"{highScore.Score}");
                }

                await Breakout.Instance.FileSystem.Write("breakout.lst", scoresStr.ToString());

                await Breakout.Instance.StateMachine.Change("high-scores", new Dictionary<string, object>
                {
                    ["highScores"] = highScores
                });
            }

            // scroll through character slots
            if (Breakout.Instance.Keyboard.WasPressed(Key.Left) && highlightedChar > 0)
            {
                highlightedChar--;
                await Breakout.Instance.Sounds["select"].Play();
            }
            else if (Breakout.Instance.Keyboard.WasPressed(Key.Right) && highlightedChar < 3 - 1)
            {
                highlightedChar++;
                await Breakout.Instance.Sounds["select"].Play();
            }

            // scroll through characters
            if (Breakout.Instance.Keyboard.WasPressed(Key.Up))
            {
                chars[highlightedChar]++;
                if (chars[highlightedChar] > 90)
                {
                    chars[highlightedChar] = (char)65;
                }
            }
            else if (Breakout.Instance.Keyboard.WasPressed(Key.Down))
            {
                chars[highlightedChar]--;
                if (chars[highlightedChar] < 65)
                {
                    chars[highlightedChar] = (char)90;
                }
            }
        }

        public override Task Render()
        {
            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["medium"]);
            Breakout.Instance.Graphics.Print($"Your score: {score}", 0, 30, Breakout.Instance.VirtualWidth, Alignment.Center);

            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["large"]);

            // render all three characters of the name
            if (highlightedChar == 0)
            {
                Breakout.Instance.Graphics.SetColor(103, 255, 255);
            }
            Breakout.Instance.Graphics.Print($"{chars[0]}", Breakout.Instance.VirtualWidth / 2 - 28, Breakout.Instance.VirtualHeight / 2);
            Breakout.Instance.Graphics.SetColor(255, 255, 255);

            if (highlightedChar == 1)
            {
                Breakout.Instance.Graphics.SetColor(103, 255, 255);
            }
            Breakout.Instance.Graphics.Print($"{chars[1]}", Breakout.Instance.VirtualWidth / 2 - 6, Breakout.Instance.VirtualHeight / 2);
            Breakout.Instance.Graphics.SetColor(255, 255, 255);

            if (highlightedChar == 2)
            {
                Breakout.Instance.Graphics.SetColor(103, 255, 255);
            }
            Breakout.Instance.Graphics.Print($"{chars[2]}", Breakout.Instance.VirtualWidth / 2 + 20, Breakout.Instance.VirtualHeight / 2);
            Breakout.Instance.Graphics.SetColor(255, 255, 255);

            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["small"]);
            Breakout.Instance.Graphics.Print("Press Enter to confirm!", 0, Breakout.Instance.VirtualHeight - 18, Breakout.Instance.VirtualWidth, Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

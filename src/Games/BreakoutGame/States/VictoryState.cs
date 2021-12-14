using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;

namespace BreakoutGame.States
{
    // StartState Class

    // Represents the state that the game is in when we've just completed a level.
    // Very similar to the ServeState, except here we increment the level 
    public class VictoryState : State
    {
        int level;
        int score;
        ScoreData[] highScores;
        Paddle paddle;
        int health;
        Ball ball;
        int recoverPoints;

        LevelMaker levelMaker;

        public VictoryState()
        {
            levelMaker = new LevelMaker();
        }

        public override Task Enter(Dictionary<string, object> parameters)
        {
            level = (int)parameters["level"];
            score = (int)parameters["score"];
            highScores = (ScoreData[])parameters["highScores"];
            paddle = (Paddle)parameters["paddle"];
            health = (int)parameters["health"];
            ball = (Ball)parameters["ball"];
            recoverPoints = (int)parameters["recoverPoints"];

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            paddle.Update(dt);

            // have the ball track the player
            ball.X = paddle.X + (paddle.Width / 2) - 4;
            ball.Y = paddle.Y - 8;

            // go to play screen if the player presses Enter
            if (Breakout.Instance.Keyboard.WasPressed(Key.Enter) || Breakout.Instance.Keyboard.WasPressed(Key.Return))
            {
                await Breakout.Instance.StateMachine.Change("serve", new Dictionary<string, object>
                {
                    ["level"] = level + 1,
                    ["bricks"] = levelMaker.CreateMap(level + 1),
                    ["paddle"] = paddle,
                    ["health"] = health,
                    ["score"] = score,
                    ["highScores"] = highScores,
                    ["recoverPoints"] = recoverPoints
                });
            }
        }

        public override Task Render()
        {
            paddle.Render();
            ball.Render();

            Breakout.Instance.RenderHealth(health);
            Breakout.Instance.RenderScore(score);

            // level complete text
            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["large"]);
            Breakout.Instance.Graphics.Print($"Level: {level} complete!", 0, Breakout.Instance.VirtualHeight / 4, Breakout.Instance.VirtualWidth, Alignment.Center);

            // instructions text
            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["medium"]);
            Breakout.Instance.Graphics.Print("Press Enter to serve!", 0, Breakout.Instance.VirtualHeight / 2, Breakout.Instance.VirtualWidth, Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

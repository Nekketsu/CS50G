using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreakoutGame.States
{
    // ServeState Class

    // The state in which we are waiting to serve the ball; here, we are
    // basically just moving the paddle left and right with the ball until we
    // press Enter, though everything in the actual game now should render in
    // preparation for the serve, including our current health and score, as
    // well as the level we're on.
    public class ServeState : State
    {
        Paddle paddle;
        Brick[] bricks;
        int health;
        int score;
        ScoreData[] highScores;
        int level;
        int recoverPoints;

        Ball ball;

        public override Task Enter(Dictionary<string, object> parameters)
        {
            // grab name state from params
            paddle = (Paddle)parameters["paddle"];
            bricks = (Brick[])parameters["bricks"];
            health = (int)parameters["health"];
            score = (int)parameters["score"];
            highScores = (ScoreData[])parameters["highScores"];
            level = (int)parameters["level"];
            recoverPoints = (int)parameters["recoverPoints"];

            // init new ball (random color for fun)
            ball = new Ball(Breakout.Instance.Random.Next(7));

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            // have the ball track the player
            paddle.Update(dt);
            ball.X = paddle.X + (paddle.Width / 2) - 4;
            ball.Y = paddle.Y - 8;

            if (Breakout.Instance.Keyboard.WasPressed(Key.Enter) || Breakout.Instance.Keyboard.WasPressed(Key.Return))
            {
                // pass in all important state info to the PlayState
                await Breakout.Instance.StateMachine.Change("play", new Dictionary<string, object>
                {
                    ["paddle"] = paddle,
                    ["bricks"] = bricks,
                    ["health"] = health,
                    ["score"] = score,
                    ["highScores"] = highScores,
                    ["ball"] = ball,
                    ["level"] = level,
                    ["recoverPoints"] = recoverPoints
                });
            }

            if (Breakout.Instance.Keyboard.WasPressed(Key.Escape))
            {
                // Quit
            }
        }

        public override Task Render()
        {
            paddle.Render();
            ball.Render();

            foreach (var brick in bricks)
            {
                brick.Render();
            }

            Breakout.Instance.RenderScore(score);
            Breakout.Instance.RenderHealth(health);

            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["large"]);
            Breakout.Instance.Graphics.Print($"Level {level}", 0, Breakout.Instance.VirtualHeight / 3, Breakout.Instance.VirtualWidth, Alignment.Center);

            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["medium"]);
            Breakout.Instance.Graphics.Print("Press Enter to serve!", 0, Breakout.Instance.VirtualHeight / 2, Breakout.Instance.VirtualWidth, Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

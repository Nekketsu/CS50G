using GameEngine.Input;
using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreakoutGame.States
{
    // PlayState Class

    // Represents the state of the game in which we are actively playing;
    // player should control the paddle, with the ball actively bouncing between
    // the bricks, walls, and the paddle. If the ball goes below the paddle, then
    // the player should lose one point of health and be taken either to the Game
    // Over screen if at 0 health or the Serve screen otherwise.
    public class PlayState : State
    {
        Paddle paddle;
        Brick[] bricks;
        int health;
        int score;
        ScoreData[] highScores;
        Ball ball;
        int level;

        int recoverPoints;

        bool paused;

        // We initialize what's in our PlayState via a state table that we pass between
        // states as we go from playing to serving.
        public override Task Enter(Dictionary<string, object> parameters)
        {
            paddle = (Paddle)parameters["paddle"];
            bricks = (Brick[])parameters["bricks"];
            health = (int)parameters["health"];
            score = (int)parameters["score"];
            highScores = (ScoreData[])parameters["highScores"];
            ball = (Ball)parameters["ball"];
            level = (int)parameters["level"];
            recoverPoints = (int)parameters["recoverPoints"];

            // give ball random starting velocity
            ball.Dx = Breakout.Instance.Random.Next(-200, 200 + 1);
            ball.Dy = -50; // Breakout.Instance.Random.Next(-60, -50 + 1);

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            if (paused)
            {
                if (Breakout.Instance.Keyboard.WasPressed(Key.Space))
                {
                    paused = false;
                    await Breakout.Instance.Sounds["pause"].Play();
                }
                else
                {
                    return;
                }
            }

            // update positions based on velocity
            paddle.Update(dt);
            await ball.Update(dt);

            if (ball.Collides(paddle))
            {
                // raise ball above paddle in case it goes below it, then reverse dy
                ball.Y = paddle.Y - 8;
                ball.Dy = -ball.Dy;

                // tweak angle of bounce based on where it hits the paddle

                // if we hit the paddle on its left side while moving left...
                if (ball.X < paddle.X + (paddle.Width / 2) && paddle.X < 0)
                {
                    ball.Dx = -50 - (8 * (paddle.X + paddle.Width / 2 - ball.X));
                }

                // else if we hit the paddle on its right side while moving right...
                else if (ball.X > paddle.X + (paddle.Width / 2) && paddle.Dx > 0)
                {
                    ball.Dx = 50 + (8 * Math.Abs(paddle.X + paddle.Width / 2 - ball.X));
                }

                await Breakout.Instance.Sounds["paddle-hit"].Play();
            }

            // detect collision across all bricks with the ball
            foreach (var brick in bricks)
            {
                // only check collision if we're in play
                if (brick.InPlay && ball.Collides(brick))
                {
                    // add to score
                    score += (brick.Tier * 200 + (brick.Color + 1) * 25);

                    // trigger the brick's hit function, which removes it from play
                    brick.Hit();

                    // if we have enough points, recover a point of health
                    if (score > recoverPoints)
                    {
                        // can't go above 3 health
                        health = Math.Min(3, health + 1);

                        // multiply recovery points by 2
                        recoverPoints = Math.Min(100000, recoverPoints * 2);

                        // play recover sound effect
                        await Breakout.Instance.Sounds["recover"].Play();
                    }

                    // go to our victory screen if there are no more bricks left
                    if (CheckVictory())
                    {
                        await Breakout.Instance.Sounds["victory"].Play();

                        await Breakout.Instance.StateMachine.Change("victory", new Dictionary<string, object>
                        {
                            ["level"] = level,
                            ["paddle"] = paddle,
                            ["health"] = health,
                            ["score"] = score,
                            ["highScores"] = highScores,
                            ["ball"] = ball,
                            ["recoverPoints"] = recoverPoints
                        });
                    }

                    // collision code for bricks

                    // we check to see if the opposite side of our velocity is outside of the brick;
                    // if it is, we trigger a collision on that side. else we're within the X + width of
                    // the brick and should check to see if the top or bottom edge is outside of the brick,
                    // colliding on the top or bottom accordingly 


                    // left edge; only check if we're moving right, and offset the check by a couple of pixels
                    // so that flush corner hits register as Y flips, not X flips
                    if (ball.X + 2 < brick.X && ball.Dx > 0)
                    {
                        // flip x velocity and reset position outside of brick
                        ball.Dx = -ball.Dx;
                        ball.X = brick.X - 8;
                    }
                    // right edge; only check if we're moving left, , and offset the check by a couple of pixels
                    // so that flush corner hits register as Y flips, not X flips
                    else if (ball.X + 6 > brick.X + brick.Width && ball.Dy < 0)
                    {
                        // flip y velocity and reset position outside of brick
                        ball.Dx = -ball.Dx;
                        ball.X = brick.X + 32;
                    }
                    // top edge if no X collisions, always check
                    else if (ball.Y < brick.Y)
                    {
                        // flip y velocity and reset position outside of brick
                        ball.Dy = -ball.Dy;
                        ball.Y = brick.Y - 8;
                    }
                    // bottom edge if no X collisions or top collision, last possibility
                    else
                    {
                        // flip y velocity and reset position outside of brick
                        ball.Dy = -ball.Dy;
                        ball.Y = brick.Y + 16;
                    }

                    // slightly scale the y velocity to speed up the game, capping at +- 150
                    if (Math.Abs(ball.Dy) < 150)
                    {
                        ball.Dy *= 1.02;
                    }

                    // only allow colliding with one brick, for corners
                    break;
                }
            }

            // if ball goes below bounds, revert to serve state and decrease health
            if (ball.Y >= Breakout.Instance.VirtualHeight)
            {
                health--;
                await Breakout.Instance.Sounds["hurt"].Play();

                if (health == 0)
                {
                    await Breakout.Instance.StateMachine.Change("game-over", new Dictionary<string, object>
                    {
                        ["score"] = score,
                        ["highScores"] = highScores
                    });
                }
                else
                {
                    await Breakout.Instance.StateMachine.Change("serve", new Dictionary<string, object>
                    {
                        ["paddle"] = paddle,
                        ["bricks"] = bricks,
                        ["health"] = health,
                        ["score"] = score,
                        ["highScores"] = highScores,
                        ["level"] = level,
                        ["recoverPoints"] = recoverPoints
                    });
                }
            }

            // for rendering particle systems
            foreach (var brick in bricks)
            {
                brick.Update(dt);
            }

            if (Breakout.Instance.Keyboard.WasPressed(Key.Escape))
            {
                // Quit
            }
        }

        public override Task Render()
        {
            // render bricks
            foreach (var brick in bricks)
            {
                brick.Render();
            }

            //// render all particle systems
            //foreach (var brick in bricks)
            //{
            //    brick.RenderParticles();
            //}

            paddle.Render();
            ball.Render();

            Breakout.Instance.RenderScore(score);
            Breakout.Instance.RenderHealth(health);

            // pause text, if paused
            if (paused)
            {
                Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["large"]);
                Breakout.Instance.Graphics.Print("PAUSED", 0, Breakout.Instance.VirtualHeight / 2 - 16, Breakout.Instance.VirtualWidth, GameEngine.Graphics.Alignment.Center);
            }

            return Task.CompletedTask;
        }

        private bool CheckVictory()
        {
            foreach (var brick in bricks)
            {
                if (brick.InPlay)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

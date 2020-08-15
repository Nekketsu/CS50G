using GameEngine;
using GameEngine.Audio;
using GameEngine.Graphics;
using GameEngine.Input;
using System;
using System.Threading.Tasks;

namespace PongGame
{
    // Main Program

    // Originally programmed by Atari in 1972. Features two
    // paddles, controlled by players, with the goal of getting
    // the ball past your opponent's edge. First to 10 points wins.

    // This version is built to more closely resemble the NES than
    // the original Pong machines or the Atari 2600 in terms of
    // resolution, though in widescreen (16:9) so it looks nicer on 
    // modern systems.
    public class Pong : Game
    {
        public static new Pong Instance { get; private set; }

        public Pong(IGameLoop gameLoop, IGraphics graphics, IAudio audio, IKeyboard keyboard) : base(nameof(Pong), gameLoop, graphics, audio, keyboard)
        {
            Instance = this;
        }

        public override int VirtualWidth => 432;
        public override int VirtualHeight => 243;

        private const int PaddleSpeed = 200;

        Random random;

        Font smallFont;
        Font largeFont;
        Font scoreFont;

        Paddle player1;
        Paddle player2;

        Ball ball;

        int player1Score;
        int player2Score;

        int servingPlayer;

        int winningPlayer;

        GameState gameState;

        public override async Task Load()
        {
            // set love's default filter to "nearest-neighbor", which essentially
            // means there will be no filtering of pixels(blurriness), which is
            // important for a nice crisp, 2D look
            // love.graphics.setDefaultFilter('nearest', 'nearest')
            // love.window.setTitle('Pong')

            random = new Random();

            // initialize our nice-looking retro text fonts
            smallFont = await Graphics.NewFont("font.ttf", 8);
            largeFont = await Graphics.NewFont("font.ttf", 16);
            scoreFont = await Graphics.NewFont("font.ttf", 32);
            Graphics.SetFont(smallFont);

            // Sounds
            Sounds.Add("paddle_hit", await Audio.NewSource("sounds/paddle_hit.wav"));
            Sounds.Add("score", await Audio.NewSource("sounds/score.wav"));
            Sounds.Add("wall_hit", await Audio.NewSource("sounds/wall_hit.wav"));

            // initialize our player paddles; make them global so that they can be
            // detected by other functions and modules
            player1 = new Paddle(10, 30, 5, 20);
            player2 = new Paddle(VirtualWidth - 10, VirtualHeight - 30, 5, 20);

            // place a ball in the middle of the screen
            ball = new Ball(VirtualWidth / 2 - 2, VirtualHeight / 2 - 2, 4, 4);

            // initialize score variables
            player1Score = 0;
            player2Score = 0;

            // either going to be 1 or 2; whomever is scored on gets to serve the
            // following turn
            servingPlayer = 1;

            // player who won the game; not set to a proper value until we reach
            // that state in the game
            winningPlayer = 0;

            gameState = GameState.Start;
        }

        // Called every frame, passing in `dt` since the last frame. `dt`
        // is short for `deltaTime` and is measured in seconds.Multiplying
        // this by any changes we wish to make in our game will allow our
        // game to perform consistently across all hardware; otherwise, any
        // changes we make will be applied as fast as possible and will vary
        // across system hardware.
        public override async Task Update(TimeSpan dt)
        {
            switch (gameState)
            {
                case GameState.Serve:
                    // before switching to play, initialize ball's velocity based
                    // on player who last scored
                    ball.Dy = random.Next(-50, 50);
                    if (servingPlayer == 1)
                    {
                        ball.Dx = random.Next(140, 200);
                    }
                    else
                    {
                        ball.Dx = -random.Next(140, 200);
                    }

                    break;
                case GameState.Play:
                    // detect ball collision with paddles, reversing dx if true and
                    // slightly increasing it, then altering the dy based on the position
                    // at which it collided, then playing a sound effect
                    if (ball.Collides(player1))
                    {
                        ball.Dx = -ball.Dx * 1.03;
                        ball.X = player1.X + 5;

                        // keep velocity going in the same direction, but randomize it
                        if (ball.Dy < 0)
                        {
                            ball.Dy = -random.Next(10, 150);
                        }
                        else
                        {
                            ball.Dy = random.Next(10, 150);
                        }

                        await Sounds["paddle_hit"].Play();
                    }
                    if (ball.Collides(player2))
                    {
                        ball.Dx = -ball.Dx * 1.03;
                        ball.X = player2.X - 4;

                        // keep velocity going in the same direction, but randomize it
                        if (ball.Dy < 0)
                        {
                            ball.Dy = -random.Next(10, 150);
                        }
                        else
                        {
                            ball.Dy = random.Next(10, 150);
                        }

                        await Sounds["paddle_hit"].Play();
                    }

                    // detect upper and lower screen boundary collision, playing a sound
                    // effect and reversing dy if true
                    if (ball.Y <= 0)
                    {
                        ball.Y = 0;
                        ball.Dy = -ball.Dy;
                        await Sounds["wall_hit"].Play();
                    }

                    // -4 to account for the ball's size
                    if (ball.Y >= VirtualHeight - 4)
                    {
                        ball.Y = VirtualHeight - 4;
                        ball.Dy = -ball.Dy;
                        await Sounds["wall_hit"].Play();
                    }

                    // if we reach the left edge of the screen, go back to serve
                    // and update the score and serving player
                    if (ball.X < 0)
                    {
                        servingPlayer = 1;
                        player2Score++;
                        await Sounds["score"].Play();

                        // if we've reached a score of 10, the game is over; set the
                        // state to done so we can show the victory message
                        if (player2Score == 10)
                        {
                            winningPlayer = 2;
                            gameState = GameState.Done;
                        }
                        else
                        {
                            gameState = GameState.Serve;
                            // places the ball in the middle of the screen, no velocity
                            ball.Reset();
                        }
                    }

                    // if we reach the right edge of the screen, go back to serve
                    // and update the score and serving player
                    if (ball.X > VirtualWidth)
                    {
                        servingPlayer = 2;
                        player1Score++;
                        await Sounds["score"].Play();

                        // if we've reached a score of 10, the game is over; set the
                        // state to done so we can show the victory message
                        if (player1Score == 10)
                        {
                            winningPlayer = 1;
                            gameState = GameState.Done;
                        }
                        else
                        {
                            gameState = GameState.Serve;
                            // places the ball in the middle of the screen, no velocity
                            ball.Reset();
                        }
                    }

                    break;
            }

            //
            // paddles can move no matter what state we're in
            //
            // player 1
            if (Keyboard.IsDown(Key.W))
            {
                player1.Dy = -PaddleSpeed;
            }
            else if (Keyboard.IsDown(Key.S))
            {
                player1.Dy = PaddleSpeed;
            }
            else
            {
                player1.Dy = 0;
            }

            // player 2
            if (Keyboard.IsDown(Key.Up))
            {
                player2.Dy = -PaddleSpeed;
            }
            else if (Keyboard.IsDown(Key.Down))
            {
                player2.Dy = PaddleSpeed;
            }
            else
            {
                player2.Dy = 0;
            }

            // update our ball based on its DX and DY only if we're in play state;
            // scale the velocity by dt so movement is framerate - independent
            if (gameState == GameState.Play)
            {
                ball.Update(dt);
            }

            player1.Update(dt);
            player2.Update(dt);
        }

        // A callback that processes key strokes as they happen, just the once.
        // Does not account for keys that are held down, which is handled by a
        // separate function (`love.keyboard.isDown`). Useful for when we want
        // things to happen right away, just once, like when we want to quit.
        public override void KeyPressed(Key key)
        {
            // `key` will be whatever key this callback detected as pressed
            if (key == Key.Escape)
            {
                // Quit
            }
            else if (key == Key.Enter || key == Key.Return)
            {
                switch (gameState)
                {
                    case GameState.Start:
                        gameState = GameState.Serve;
                        break;
                    case GameState.Serve:
                        gameState = GameState.Play;
                        break;
                    case GameState.Done:
                        // game is simply in a restart phase here, but will set the serving
                        // player to the opponent of whomever won for fairness!
                        gameState = GameState.Serve;

                        ball.Reset();

                        // reset scores to 0
                        player1Score = 0;
                        player2Score = 0;

                        // decide serving player as the opposite of who won
                        if (winningPlayer == 1)
                        {
                            servingPlayer = 2;
                        }
                        else
                        {
                            servingPlayer = 1;
                        }
                        break;
                    case GameState.Play:
                        break;
                    default:
                        break;
                }
            }
        }

        // Called each frame after update; is responsible simply for
        // drawing all of our game objects and more to the screen.
        public override async Task Draw()
        {
            Graphics.Begin();

            // begin drawing with push, in our virtual resolution
            Graphics.Clear(40, 45, 52);

            Graphics.SetColor(255, 255, 255);

            // render different things depending on which part of the game we're in
            if (gameState == GameState.Start)
            {
                // UI messages
                Graphics.SetFont(smallFont);
                Graphics.Print("Wellcome to Pong!", 0, 10, VirtualWidth, Alignment.Center);
                Graphics.Print("Press Enter to begin!", 0, 20, VirtualWidth, Alignment.Center);
            }
            else if (gameState == GameState.Serve)
            {
                // UI messages
                Graphics.SetFont(smallFont);
                Graphics.Print($"Player {servingPlayer}\\'s serve!", 0, 10, VirtualWidth, Alignment.Center);
                Graphics.Print("Press Enter to serve!", 0, 20, VirtualWidth, Alignment.Center);
            }
            else if (gameState == GameState.Play)
            {
                // No UI messages to display in play
            }
            else if (gameState == GameState.Done)
            {
                // UI messages
                Graphics.SetFont(largeFont);
                Graphics.Print($"Player {winningPlayer} wins!", 0, 10, VirtualWidth, Alignment.Center);
                Graphics.SetFont(smallFont);
                Graphics.Print("Press Enter to restart!", 0, 30, VirtualWidth, Alignment.Center);
            }

            // show the score before ball is rendered so it can move over the text
            DisplayScore();

            player1.Render();
            player2.Render();
            ball.Render();

            // Display FPS for debugging; simply comment out to remove
            // DisplayFps();

            await Graphics.Apply();
        }

        // Simple function for rendering the scores.
        public void DisplayScore()
        {
            // score display
            Graphics.SetFont(scoreFont);
            Graphics.Print($"{player1Score}", VirtualWidth / 2 - 50, VirtualHeight / 3);
            Graphics.Print($"{player2Score}", VirtualWidth / 2 + 30, VirtualHeight / 3);
        }

        // Renders the current FPS.
        public void DisplayFps()
        {
            // simple FPS display across all states
            Graphics.SetFont(smallFont);
            Graphics.SetColor(0, 255, 0);
            // Graphics.Print("FPS: " + Game.Timer.Fps);
        }
    }
}

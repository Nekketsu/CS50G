using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;

namespace BreakoutGame.States
{
    // StartState Class

    // Represents the state the game is in when we've just started; should
    // simply display "Breakout" in large text, as well as a message to press
    // Enter to begin.
    public class PaddleSelectState : State
    {
        ScoreData[] highScores;

        int currentPaddle;

        LevelMaker levelMaker;

        public PaddleSelectState()
        {
            // the paddle we're highlighting; will be passed to the ServeState
            // when we press Enter
            currentPaddle = 0;

            levelMaker = new LevelMaker();
        }

        public override Task Enter(Dictionary<string, object> parameters)
        {
            highScores = (ScoreData[])parameters["highScores"];

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            if (Breakout.Instance.Keyboard.WasPressed(Key.Left))
            {
                if (currentPaddle == 0)
                {
                    await Breakout.Instance.Sounds["no-select"].Play();
                }
                else
                {
                    await Breakout.Instance.Sounds["select"].Play();
                    currentPaddle--;
                }
            }
            else if (Breakout.Instance.Keyboard.WasPressed(Key.Right))
            {
                if (currentPaddle == 3)
                {
                    await Breakout.Instance.Sounds["no-select"].Play();
                }
                else
                {
                    await Breakout.Instance.Sounds["select"].Play();
                    currentPaddle++;
                }
            }

            // select paddle and move on to the serve state, passing in the selection
            if (Breakout.Instance.Keyboard.WasPressed(Key.Enter) || Breakout.Instance.Keyboard.WasPressed(Key.Return))
            {
                await Breakout.Instance.Sounds["confirm"].Play();

                await Breakout.Instance.StateMachine.Change("serve", new Dictionary<string, object>
                {
                    ["paddle"] = new Paddle(currentPaddle),
                    ["bricks"] = levelMaker.CreateMap(1),
                    ["health"] = 3,
                    ["score"] = 0,
                    ["highScores"] = highScores,
                    ["level"] = 1,
                    ["recoverPoints"] = 5000
                });
            }

            if (Breakout.Instance.Keyboard.WasPressed(Key.Escape))
            {
                // Quit
            }
        }

        public override Task Render()
        {
            // instructions
            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["medium"]);
            Breakout.Instance.Graphics.Print("Select your paddle with left and right!", 0, Breakout.Instance.VirtualHeight / 4, Breakout.Instance.VirtualWidth, Alignment.Center);

            // left arrow; should render normally if we're higher than 1, else
            // in a shadowy form to let us know we're as far left as we can go
            if (currentPaddle == 0)
            {
                // tint; give it a dark gray with half opacity
                Breakout.Instance.Graphics.SetColor(40, 40, 40, 128);
            }

            Breakout.Instance.Graphics.Draw(Breakout.Instance.Textures["arrows"], Breakout.Instance.Frames["arrows"][0], Breakout.Instance.VirtualWidth / 4 - 24, Breakout.Instance.VirtualHeight - Breakout.Instance.VirtualHeight / 3);

            // reset drawing color to full white for proper rendering
            Breakout.Instance.Graphics.SetColor(255, 255, 255, 255);

            // right arrow; should render normally if we're less than 4, else
            // in a shadowy form to let us know we're as far right as we can go
            if (currentPaddle == 3)
            {
                // tint; give it a dark gray with half opacity
                Breakout.Instance.Graphics.SetColor(40, 40, 40, 128);
            }

            Breakout.Instance.Graphics.Draw(Breakout.Instance.Textures["arrows"], Breakout.Instance.Frames["arrows"][1], Breakout.Instance.VirtualWidth - Breakout.Instance.VirtualWidth / 4, Breakout.Instance.VirtualHeight - Breakout.Instance.VirtualHeight / 3);

            // reset drawing color to full white for proper rendering
            Breakout.Instance.Graphics.SetColor(255, 255, 255, 255);

            // draw the paddle itself, based on which we have selected
            Breakout.Instance.Graphics.Draw(Breakout.Instance.Textures["main"], Breakout.Instance.Frames["paddles"][1 + 4 * currentPaddle], Breakout.Instance.VirtualWidth / 2 - 32, Breakout.Instance.VirtualHeight - Breakout.Instance.VirtualHeight / 3);

            return Task.CompletedTask;
        }
    }
}

using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreakoutGame.States
{
    // StartState Class

    // Represents the state the game is in when we've just started; should
    // simply display "Breakout" in large text, as well as a message to press
    // Enter to begin.
    public class StartState : State
    {
        // whether we're highlighting "Start" or "High Scores"
        int highlighted = 1;

        ScoreData[] highScores;

        public override Task Enter(Dictionary<string, object> parameters)
        {
            highScores = (ScoreData[])parameters["highScores"];

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            // toggle highlighted option if we press an arrow key up or down
            if (Breakout.Instance.Keyboard.WasPressed(Key.Up) || Breakout.Instance.Keyboard.WasPressed(Key.Down))
            {
                highlighted = highlighted == 1 ? 2 : 1;
                await Breakout.Instance.Sounds["paddle-hit"].Play();
            }

            // confirm whichever option we have selected to change screens
            if (Breakout.Instance.Keyboard.WasPressed(Key.Enter) || Breakout.Instance.Keyboard.WasPressed(Key.Return))
            {
                await Breakout.Instance.Sounds["confirm"].Play();

                if (highlighted == 1)
                {
                    await Breakout.Instance.StateMachine.Change("paddle-select", new Dictionary<string, object>
                    {
                        ["highScores"] = highScores
                    });
                }
                else
                {
                    await Breakout.Instance.StateMachine.Change("high-scores", new Dictionary<string, object>
                    {
                        ["highScores"] = highScores
                    });
                }
            }

            // we no longer have this globally, so include here
            if (Breakout.Instance.Keyboard.WasPressed(Key.Escape))
            {
                // Quit
            }
        }

        public override Task Render()
        {
            // title
            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["large"]);
            Breakout.Instance.Graphics.Print("BREAKOUT", 0, Breakout.Instance.VirtualHeight / 3, Breakout.Instance.VirtualWidth, Alignment.Center);

            // instructions
            Breakout.Instance.Graphics.SetFont(Breakout.Instance.Fonts["medium"]);

            // if we're highlighting 1, render that option blue
            if (highlighted == 1)
            {
                Breakout.Instance.Graphics.SetColor(103, 255, 255, 255);
            }
            Breakout.Instance.Graphics.Print("START", 0, Breakout.Instance.VirtualHeight / 2 + 70, Breakout.Instance.VirtualWidth, Alignment.Center);

            // reset the color
            Breakout.Instance.Graphics.SetColor(255, 255, 255, 255);

            // render option 2 blue if we're highlighting that one
            if (highlighted == 2)
            {
                Breakout.Instance.Graphics.SetColor(103, 255, 255, 255);
            }
            Breakout.Instance.Graphics.Print("HIGH SCORES", 0, Breakout.Instance.VirtualHeight / 2 + 90, Breakout.Instance.VirtualWidth, Alignment.Center);

            // reset the color
            Breakout.Instance.Graphics.SetColor(255, 255, 255, 255);

            return Task.CompletedTask;
        }
    }
}

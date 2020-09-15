using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using System;
using System.Threading.Tasks;

namespace LegendOf50Game.States.Game
{
    // StartState Class
    public class StartState : State
    {
        public override async Task Update(TimeSpan dt)
        {
            if (LegendOf50.Instance.Keyboard.WasPressed(Key.Escape))
            {
                // Quit
            }

            if (LegendOf50.Instance.Keyboard.WasPressed(Key.Enter) || LegendOf50.Instance.Keyboard.WasPressed(Key.Return))
            {
                await LegendOf50.Instance.StateMachine.Change("play");
            }
        }

        public override Task Render()
        {
            LegendOf50.Instance.Graphics.Draw(LegendOf50.Instance.Textures["background"], 0, 0,
                (double)LegendOf50.Instance.VirtualWidth / LegendOf50.Instance.Textures["background"].Width,
                (double)LegendOf50.Instance.VirtualHeight / LegendOf50.Instance.Textures["background"].Height);

            //LegendOf50.Instance.Graphics.SetFont(LegendOf50.Instance.Fonts["gothic-medium"]);
            //LegendOf50.Instance.Graphics.Print("Legend of", 0, LegendOf50.Instance.VirtualHeight / 2 - 32, LegendOf50.Instance.VirtualWidth, Alignment.Center);

            //LegendOf50.Instance.Graphics.SetFont(LegendOf50.Instance.Fonts["gothic-large"]);
            //LegendOf50.Instance.Graphics.Print("50", 0, LegendOf50.Instance.VirtualHeight / 2 - 16, LegendOf50.Instance.VirtualWidth, Alignment.Center);

            LegendOf50.Instance.Graphics.SetFont(LegendOf50.Instance.Fonts["zelda"]);
            LegendOf50.Instance.Graphics.SetColor(34, 34, 34, 255);
            LegendOf50.Instance.Graphics.Print("Legend of 50", 2, LegendOf50.Instance.VirtualHeight / 2 - 30, LegendOf50.Instance.VirtualWidth, Alignment.Center);

            LegendOf50.Instance.Graphics.SetColor(175, 53, 42, 255);
            LegendOf50.Instance.Graphics.Print("Legend of 50", 0, LegendOf50.Instance.VirtualHeight / 2 - 32, LegendOf50.Instance.VirtualWidth, Alignment.Center);

            LegendOf50.Instance.Graphics.SetColor(255, 255, 255, 255);
            LegendOf50.Instance.Graphics.SetFont(LegendOf50.Instance.Fonts["zelda-small"]);
            LegendOf50.Instance.Graphics.Print("Press Enter", 0, LegendOf50.Instance.VirtualHeight / 2 + 64, LegendOf50.Instance.VirtualWidth, Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

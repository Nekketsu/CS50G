using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using System;
using System.Threading.Tasks;

namespace LegendOf50Game.States.Game
{
    // GameOverState Class
    public class GameOverState : State
    {
        public override async Task Update(TimeSpan dt)
        {
            if (LegendOf50.Instance.Keyboard.WasPressed(Key.Enter) || LegendOf50.Instance.Keyboard.WasPressed(Key.Return))
            {
                await LegendOf50.Instance.StateMachine.Change("start");
            }

            if (LegendOf50.Instance.Keyboard.WasPressed(Key.Escape))
            {
                // Quit
            }
        }

        public override Task Render()
        {
            LegendOf50.Instance.Graphics.SetFont(LegendOf50.Instance.Fonts["zelda"]);
            LegendOf50.Instance.Graphics.SetColor(175, 53, 42, 255);
            LegendOf50.Instance.Graphics.Print("GAME OVER", 0, LegendOf50.Instance.VirtualHeight / 2 - 48, LegendOf50.Instance.VirtualWidth, Alignment.Center);

            LegendOf50.Instance.Graphics.SetFont(LegendOf50.Instance.Fonts["zelda-small"]);
            LegendOf50.Instance.Graphics.Print("Press Enter", 0, LegendOf50.Instance.VirtualHeight / 2 + 16, LegendOf50.Instance.VirtualWidth, Alignment.Center);
            LegendOf50.Instance.Graphics.SetColor(255, 255, 255, 255);

            return Task.CompletedTask;
        }
    }
}

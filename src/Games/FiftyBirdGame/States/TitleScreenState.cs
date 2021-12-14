using GameEngine.Input;
using GameEngine.States;

namespace FiftyBirdGame.States
{
    // TitleScreenState Class

    // The TitleScreenState is the starting screen of the game, shown on startup.It should
    // display "Press Enter" and also our highest score.
    public class TitleScreenState : State
    {
        public async override Task Update(TimeSpan dt)
        {
            // transition to countdown when enter/return are pressed
            if (FiftyBird.Instance.Keyboard.WasPressed(Key.Enter) || FiftyBird.Instance.Keyboard.WasPressed(Key.Return))
            {
                await FiftyBird.Instance.StateMachine.Change("countdown");
            }
        }

        public override Task Render()
        {
            // simple UI code
            FiftyBird.Instance.Graphics.SetFont(FiftyBird.Instance.FlappyFont);
            FiftyBird.Instance.Graphics.Print($"Fifty Bird", 0, 64, FiftyBird.Instance.VirtualWidth, GameEngine.Graphics.Alignment.Center);

            FiftyBird.Instance.Graphics.SetFont(FiftyBird.Instance.MediumFont);
            FiftyBird.Instance.Graphics.Print("Press Enter", 0, 100, FiftyBird.Instance.VirtualWidth, GameEngine.Graphics.Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;

namespace FiftyBirdGame.States
{
    // ScoreState Class

    // A simple state used to display the player's score before they
    // transition back into the play state. Transitioned to from the
    // PlayState when they collide with a Pipe.
    public class ScoreState : State
    {
        int score;

        public override Task Enter(Dictionary<string, object> parameters)
        {
            score = (int)parameters["score"];

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            // go back to play if enter is pressed
            if (FiftyBird.Instance.Keyboard.WasPressed(Key.Enter) || FiftyBird.Instance.Keyboard.WasPressed(Key.Return))
            {
                await FiftyBird.Instance.StateMachine.Change("countdown");
            }
        }

        public override Task Render()
        {
            // simply render the score to the middle of the screen
            FiftyBird.Instance.Graphics.SetFont(FiftyBird.Instance.FlappyFont);
            FiftyBird.Instance.Graphics.Print("Oof! You lost!", 0, 64, FiftyBird.Instance.VirtualWidth, Alignment.Center);

            FiftyBird.Instance.Graphics.SetFont(FiftyBird.Instance.MediumFont);
            FiftyBird.Instance.Graphics.Print($"Score: {score}", 0, 100, FiftyBird.Instance.VirtualWidth, Alignment.Center);

            FiftyBird.Instance.Graphics.Print("Press Enter to Play Again!", 0, 160, FiftyBird.Instance.VirtualWidth, Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

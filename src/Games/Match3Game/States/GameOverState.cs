using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;

namespace Match3Game.States
{
    // GameOverState Class

    // State that simply shows us our score when we finally lose.
    public class GameOverState : State
    {
        int score;

        public override Task Enter(Dictionary<string, object> parameters)
        {
            score = (int)parameters["score"];

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            if (Match3.Instance.Keyboard.WasPressed(Key.Enter) || Match3.Instance.Keyboard.WasPressed(Key.Return))
            {
                await Match3.Instance.StateMachine.Change("start");
            }
        }

        public override Task Render()
        {
            Match3.Instance.Graphics.SetFont(Match3.Instance.Fonts["large"]);

            Match3.Instance.Graphics.SetColor(56, 56, 56, 234);
            Match3.Instance.Graphics.Rectangle(DrawMode.Fill, Match3.Instance.VirtualWidth / 2 - 64, 64, 128, 136, 4);

            Match3.Instance.Graphics.SetColor(99, 155, 255, 255);
            Match3.Instance.Graphics.Print("GAME OVER", Match3.Instance.VirtualWidth / 2 - 64, 64, 128, Alignment.Center);
            Match3.Instance.Graphics.SetFont(Match3.Instance.Fonts["medium"]);
            Match3.Instance.Graphics.Print($"Your Score: {score}", Match3.Instance.VirtualWidth / 2 - 64, 140, 128, Alignment.Center);
            Match3.Instance.Graphics.Print("Press Enter", Match3.Instance.VirtualWidth / 2 - 64, 180, 128, Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

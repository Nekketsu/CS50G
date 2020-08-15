using FiftyBirdGame.States;
using GameEngine.Graphics;
using System;
using System.Threading.Tasks;

namespace FiftyBirdGame
{
    // Pipe Class

    // The Pipe class represents the pipes that randomly spawn in our game, which act as our primary obstacles.
    // The pipes can stick out a random distance from the top or bottom of the screen. When the player collides
    // with one of them, it's game over. Rather than our bird actually moving through the screen horizontally,
    // the pipes themselves scroll through the game to give the illusion of player movement.
    public class Pipe
    {
        Image pipeImage;

        public double X { get; set; }
        public double Y { get; set; }

        public int Width { get; }
        public int Height { get; }

        private Orientation orientation;

        public Pipe(Orientation orientation, double y)
        {
            X = FiftyBird.Instance.VirtualWidth + 64;
            Y = y;

            Width = PlayState.PipeWidth;
            Height = PlayState.PipeHeight;

            this.orientation = orientation;
        }

        public async Task Load()
        {
            pipeImage = await FiftyBird.Instance.Graphics.NewImage("pipe.png");
        }

        public void Update(TimeSpan dt)
        {

        }

        public void Render()
        {
            FiftyBird.Instance.Graphics.Draw(pipeImage, X,
                orientation == Orientation.Top ? Y + PlayState.PipeHeight : Y,
                false, orientation == Orientation.Top);
        }
    }
}

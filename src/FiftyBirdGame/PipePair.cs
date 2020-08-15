using FiftyBirdGame.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiftyBirdGame
{
    // PipePair Class

    // Used to represent a pair of pipes that stick together as they scroll, providing an opening
    // for the player to jump through in order to score a point.
    public class PipePair
    {
        const int GapHeight = 90;

        public bool Scored { get; set; }

        public double X { get; private set; }
        public double Y { get; private set; }

        public Dictionary<string, Pipe> Pipes { get; }

        public bool Remove { get; private set; }

        public PipePair(double y)
        {
            // flag to hold whether this pair has been scored(jumped through)
            Scored = false;

            // initialize pipes past the end of the screen
            X = FiftyBird.Instance.VirtualWidth + 32;

            // y value is for the topmost pipe; gap is a vertical shift of the second lower pipe
            Y = y;

            // instantiate two pipes that belong to this pair
            Pipes = new Dictionary<string, Pipe>
            {
                ["upper"] = new Pipe(Orientation.Top, Y),
                ["lower"] = new Pipe(Orientation.Bottom, Y + PlayState.PipeHeight + GapHeight)
            };

            // whether this pipe pair is ready to be removed from the scene
            Remove = false;
        }

        public async Task Load()
        {
            //var pipeLoadTasks = Pipes.Values.Select(pipe => pipe.Load());
            //await Task.WhenAll(pipeLoadTasks);
            foreach (var pipe in Pipes.Values)
            {
                await pipe.Load();
            }
        }

        public void Update(TimeSpan dt)
        {
            // remove the pipe from the scene if it's beyond the left edge of the screen,
            // else move it from right to left
            if (X > -PlayState.PipeWidth)
            {
                X -= PlayState.PipeSpeed * dt.TotalSeconds;
                Pipes["lower"].X = X;
                Pipes["upper"].X = X;
            }
            else
            {
                Remove = true;
            }
        }

        public void Render()
        {
            foreach (var pipe in Pipes.Values)
            {
                pipe.Render();
            }
        }
    }
}

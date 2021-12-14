using GameEngine;
using GameEngine.States;

namespace FiftyBirdGame.States
{
    // PlayState Class

    // The PlayState class is the bulk of the game, where the player actually controls the bird and
    // avoids pipes.When the player collides with a pipe, we should go to the GameOver state, where
    // we then go back to the main menu.
    public class PlayState : State
    {
        public const int PipeSpeed = 60;
        public const int PipeWidth = 70;
        public const int PipeHeight = 288;

        public const int BirdWidth = 38;
        public const int BirdhHeight = 24;

        Bird bird;
        List<PipePair> pipePairs;
        double timer;
        int score;

        double lastY;

        bool scrolling;

        public override async Task Update(TimeSpan dt)
        {
            // update timer for pipe spawning
            timer = timer + dt.TotalSeconds;

            // spawn a new pipe pair every second and a half
            if (timer > 2)
            {
                // modify the last Y coordinate we placed so pipe gaps aren't too far apart
                // no higher than 10 pixels below the top edge of the screen,
                // and no lower than a gap length (90 pixels) from the bottom
                var y = Math.Max(-PipeHeight + 10, Math.Min(lastY + FiftyBird.Instance.Random.Next(-20, 20 + 1), Game.Instance.VirtualHeight));
                lastY = y;

                // add a new pipe pair at the end of the screen at our new Y
                var pipePair = new PipePair(y);
                await pipePair.Load();
                pipePairs.Add(pipePair);

                // reset timer
                timer = 0;
            }

            // for every pair of pipes..
            foreach (var pair in pipePairs)
            {
                // score a point if the pipe has gone past the bird to the left all the way
                // be sure to ignore it if it's already been scored
                if (!pair.Scored)
                {
                    if (pair.X + PipeWidth < bird.X)
                    {
                        score++;
                        pair.Scored = true;
                        await FiftyBird.Instance.Sounds["score"].Play();
                    }
                }

                // Update posiiton of pair
                pair.Update(dt);
            }

            // we need this second loop, rather than deleting in the previous loop, because
            // modifying the table in-place without explicit keys will result in skipping the
            // next pipe, since all implicit keys (numerical indices) are automatically shifted
            // down after a table removal
            pipePairs.RemoveAll(pair => pair.Remove);

            // simple collision between bird and all pipes in pairs
            foreach (var pair in pipePairs)
            {
                foreach (var pipe in pair.Pipes.Values)
                {
                    if (bird.Collides(pipe))
                    {
                        await FiftyBird.Instance.Sounds["explosion"].Play();
                        await FiftyBird.Instance.Sounds["hurt"].Play();

                        await FiftyBird.Instance.StateMachine.Change("score", new Dictionary<string, object>
                        {
                            ["score"] = score
                        });
                    }
                }
            }

            // update bird based on gravity and input
            bird.Update(dt);

            // reset if we get to the ground
            if (bird.Y > FiftyBird.Instance.VirtualHeight - 15)
            {
                await FiftyBird.Instance.Sounds["explosion"].Play();
                await FiftyBird.Instance.Sounds["hurt"].Play();

                await FiftyBird.Instance.StateMachine.Change("score", new Dictionary<string, object>
                {
                    ["score"] = score
                });

            }
        }

        public override Task Render()
        {
            foreach (var pair in pipePairs)
            {
                pair.Render();
            }

            FiftyBird.Instance.Graphics.SetFont(FiftyBird.Instance.FlappyFont);
            FiftyBird.Instance.Graphics.Print($"Score: {score}", 8, 8);

            bird.Render();

            return Task.CompletedTask;
        }

        // Called when this state is transitioned to from another state.
        public override async Task Enter(Dictionary<string, object> parameters)
        {
            bird = new Bird();
            await bird.Load();
            pipePairs = new List<PipePair>();
            timer = 0;
            score = 0;

            // initialize our last recorded Y value for a gap placement to base other gaps off of
            lastY = -PipeHeight + FiftyBird.Instance.Random.Next(80) + 20;

            // if we're coming from death, restart scrolling
            scrolling = true;
        }

        // Called when this state changes to another state.
        public override Task Exit()
        {
            // stop scrolling for the death/score screen
            scrolling = false;

            return Task.CompletedTask;
        }
    }
}

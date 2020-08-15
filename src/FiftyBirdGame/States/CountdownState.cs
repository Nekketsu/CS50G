using GameEngine.Graphics;
using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiftyBirdGame.States
{
    // Countdown State

    // Counts down visually on the screen(3,2,1) so that the player knows the
    // game is about to begin.Transitions to the PlayState as soon as the
    // countdown is complete.
    public class CountdownState : State
    {
        // takes 1 second to count down each time
        const double CountdownTime = 0.75;

        int count;
        double timer;

        public override Task Enter(Dictionary<string, object> parameters)
        {
            count = 3;
            timer = 3;

            return Task.CompletedTask;
        }

        // Keeps track of how much time has passed and decreases count if the
        // timer has exceeded our countdown time.If we have gone down to 0,
        // we should transition to our PlayState.
        public async override Task Update(TimeSpan dt)
        {
            timer += dt.TotalSeconds;

            // loop timer back to 0(plus however far past COUNTDOWN_TIME we've gone)
            // and decrement the counter once we've gone past the countdown time
            if (timer > CountdownTime)
            {
                timer = timer % CountdownTime;
                count--;

                // when 0 is reached, we should enter the PlayState
                if (count == 0)
                {
                    await FiftyBird.Instance.StateMachine.Change("play");
                }
            }
        }

        public override Task Render()
        {
            // render count big in the middle of the screen
            FiftyBird.Instance.Graphics.SetFont(FiftyBird.Instance.HugeFont);
            FiftyBird.Instance.Graphics.Print($"{count}", 0, 120, FiftyBird.Instance.VirtualWidth, Alignment.Center);

            return Task.CompletedTask;
        }
    }
}

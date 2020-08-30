using System;

namespace GameEngine.Timers
{
    public class After
    {
        TimeSpan delay;
        Action action;

        TimeSpan elapsed;

        public bool IsCompleted { get; private set; }

        public After(TimeSpan delay, Action action)
        {
            this.delay = delay;
            this.action = action;

            elapsed = TimeSpan.Zero;

            IsCompleted = false;
        }

        public void Update(TimeSpan dt)
        {
            if (!IsCompleted)
            {
                elapsed += dt;

                if (elapsed >= delay)
                {
                    action();

                    IsCompleted = true;
                }
            }
        }
    }
}

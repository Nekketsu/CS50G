using System;

namespace GameEngine.Timers
{
    public class Every
    {
        private TimeSpan interval;
        private Action action;

        private TimeSpan elapsed;

        public bool IsCompleted { get; private set; }

        public Every(TimeSpan interval, Action action)
        {
            this.interval = interval;
            this.action = action;

            elapsed = TimeSpan.Zero;

            IsCompleted = false;
        }

        public void Update(TimeSpan dt)
        {
            if (!IsCompleted)
            {
                elapsed += dt;

                while (elapsed > interval)
                {
                    action();
                    elapsed -= interval;
                }
            }
        }

        public void Remove()
        {
            IsCompleted = true;
        }
    }
}

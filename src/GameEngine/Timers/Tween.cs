using System;
using System.Threading.Tasks;

namespace GameEngine.Timers
{
    public class Tween
    {
        TimeSpan duration;
        double initialValue;
        double finalValue;

        Action<double> updateAction;
        Func<Task> finish;

        TimeSpan elapsed;
        double value;

        public bool IsCompleted { get; private set; }

        public Task<bool> Task { get; }

        public Tween(TimeSpan duration, double initialValue, double finalValue, Action<double> updateAction, Func<Task> finish = null)
        {
            this.duration = duration;
            this.initialValue = initialValue;
            this.finalValue = finalValue;

            this.updateAction = updateAction;
            this.finish = finish;

            elapsed = TimeSpan.Zero;
            value = initialValue;
            IsCompleted = false;
        }

        public async Task Update(TimeSpan dt)
        {
            if (IsCompleted) { return; }

            elapsed += dt;

            value = initialValue + (finalValue - initialValue) * Math.Min(elapsed.TotalSeconds, duration.TotalSeconds) / duration.TotalSeconds;

            updateAction?.Invoke(value);

            if (elapsed >= duration)
            {
                if (finish != null)
                {
                    IsCompleted = true;
                    await finish.Invoke();
                }
            }
        }
    }
}

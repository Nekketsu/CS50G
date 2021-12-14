namespace GameEngine.Timers
{
    public class Timer
    {
        List<Tween> tweens;
        List<After> afters;
        List<Every> everies;

        public Timer()
        {
            tweens = new List<Tween>();
            afters = new List<After>();
            everies = new List<Every>();
        }

        public Tween Tween(TimeSpan duration, double initialValue, double finalValue, Action<double> action, Func<Task> finish = null)
        {
            var tween = new Tween(duration, initialValue, finalValue, action, finish);

            tweens.Add(tween);

            return tween;
        }

        public After After(TimeSpan delay, Action action)
        {
            var after = new After(delay, action);

            afters.Add(after);

            return after;
        }

        public Every Every(TimeSpan interval, Action action)
        {
            var every = new Every(interval, action);

            everies.Add(every);

            return every;
        }

        public async Task Update(TimeSpan dt)
        {
            Tween[] theTweens;
            After[] theAfters;
            Every[] theEveries;

            theTweens = tweens.ToArray();
            theAfters = afters.ToArray();
            theEveries = everies.ToArray();

            tweens.RemoveAll(tween => tween.IsCompleted);
            afters.RemoveAll(after => after.IsCompleted);
            everies.RemoveAll(every => every.IsCompleted);

            foreach (var tween in theTweens)
            {
                await tween.Update(dt);
            }

            foreach (var after in theAfters)
            {
                after.Update(dt);
            }

            foreach (var every in theEveries)
            {
                every.Update(dt);
            }
        }

        public void Clear()
        {
            tweens.Clear();
            afters.Clear();
            everies.Clear();
        }
    }
}

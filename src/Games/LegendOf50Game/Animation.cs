namespace LegendOf50
{
    // Animation Class
    public class Animation
    {
        public int[] Frames { get; }
        public TimeSpan Interval { get; }
        public string Texture { get; }
        public bool Looping { get; }

        public TimeSpan Timer { get; private set; }
        public int CurrentFrame { get; private set; }

        public int TimesPlayed { get; set; }

        public Animation(int[] frames, TimeSpan interval, string texture = null, bool looping = true)
        {
            Frames = frames;
            Interval = interval;
            Texture = texture;
            Looping = looping;

            Timer = TimeSpan.Zero;
            CurrentFrame = 0;

            // used to see if we've seen a whole loop of the animation
            TimesPlayed = 0;
        }

        public void Refresh()
        {
            Timer = TimeSpan.Zero;
            CurrentFrame = 0;
            TimesPlayed = 0;
        }

        public void Update(TimeSpan dt)
        {
            // if not a looping animation and we've played at least once, exit
            if (!Looping && TimesPlayed > 0)
            {
                return;
            }

            // No need to update if animation is only one frame
            if (Frames.Length > 1)
            {
                Timer += dt;

                while (Timer > Interval)
                {
                    Timer -= Interval;

                    CurrentFrame = (CurrentFrame + 1) % Frames.Length;

                    // if we've looped back to the beginning, record
                    if (CurrentFrame == 0)
                    {
                        TimesPlayed++;
                    }
                }
            }
        }

        public int GetCurrentFrame()
        {
            return Frames[CurrentFrame];
        }
    }
}

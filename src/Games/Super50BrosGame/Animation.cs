namespace Super50BrosGame
{
    // Animation Class
    public class Animation
    {
        public int[] Frames { get; }
        public TimeSpan Interval { get; }
        public TimeSpan Timer { get; private set; }
        public int CurrentFrame { get; private set; }

        public Animation(int[] frames, TimeSpan interval)
        {
            Frames = frames;
            Interval = interval;
            Timer = TimeSpan.Zero;
            CurrentFrame = 0;
        }

        public void Update(TimeSpan dt)
        {
            // No need to update if animation is only one frame
            if (Frames.Length > 1)
            {
                Timer += dt;

                while (Timer > Interval)
                {
                    Timer -= Interval;

                    CurrentFrame = (CurrentFrame + 1) % Frames.Length;
                }
            }
        }

        public int GetCurrentFrame()
        {
            return Frames[CurrentFrame];
        }
    }
}

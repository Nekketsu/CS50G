namespace GameEngine.Input
{
    public class MouseButtonArgs
    {
        public int X { get; }
        public int Y { get; }
        public MouseButton Button { get; }

        public MouseButtonArgs(int x, int y, MouseButton button)
        {
            X = x;
            Y = y;
            Button = button;
        }
    }
}

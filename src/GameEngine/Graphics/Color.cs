namespace GameEngine.Graphics
{
    public class Color
    {
        public int R { get; }
        public int G { get; }
        public int B { get; }
        public int A { get; }

        public Color(int r, int g, int b) : this(r, g, b, 255) { }

        public Color(int r, int g, int b, int a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}

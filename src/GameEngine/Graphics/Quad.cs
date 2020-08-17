namespace GameEngine.Graphics
{
    public class Quad
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
        public int ReferenceImageWidth { get; }
        public int ReferencImageHeight { get; }

        public Quad(int x, int y, int width, int height, int referenceImageWidth, int referencImageHeight)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ReferenceImageWidth = referenceImageWidth;
            ReferencImageHeight = referencImageHeight;
        }
    }
}

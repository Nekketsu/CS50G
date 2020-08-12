namespace GameEngine.Graphics
{
    public class Font
    {
        public string Name { get; }
        public int Size { get; }

        public Font(string name, int size)
        {
            Name = name;
            Size = size;
        }
    }
}

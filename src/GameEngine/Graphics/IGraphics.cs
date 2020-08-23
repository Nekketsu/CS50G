using System.Threading.Tasks;

namespace GameEngine.Graphics
{
    public interface IGraphics
    {
        Task Initialize();

        Task<Font> NewFont(string fileName, int size);
        void Begin();
        Task Apply();
        void SetFont(Font font);
        Task<Image> NewImage(string fileName);
        void SetColor(int red, int green, int blue);
        void SetColor(int red, int green, int blue, int alpha);
        void Clear(int red, int green, int blue);
        void Clear(int red, int green, int blue, int alpha);
        void Print(string text, int x, int y);
        void Print(string text, int x, int y, int limit, Alignment alignment);
        void Rectangle(DrawMode mode, int x, int y, int width, int height);
        void Rectangle(DrawMode mode, double x, double y, double width, double height);
        void Draw(Image image, double x, double y);
        void Draw(Image image, double x, double y, bool flipHorizontally, bool flipVertically);
        Quad NewQuad(int x, int y, int width, int height, int referenceImageWidth, int referencImageHeight);
        void Draw(Image image, double x, double y, double scaleX, double scaleY);
        void Draw(Image image, Quad quad, double x, double y);
        void SetLineWidth(int lineWidth);
        void Rectangle(DrawMode mode, int x, int y, int width, int height, int radius);
    }
}

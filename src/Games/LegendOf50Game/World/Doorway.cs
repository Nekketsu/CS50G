namespace LegendOf50Game.World
{
    // Doorway Class
    public class Doorway
    {
        public DoorwayDirection Direction { get; }
        public bool Open { get; set; }
        public Room Room { get; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Height { get; }
        public int Width { get; }

        public Doorway(DoorwayDirection direction, bool open, Room room)
        {
            Direction = direction;
            Open = open;
            Room = room;

            switch (Direction)
            {
                case DoorwayDirection.Left:
                    X = LegendOf50.Instance.MapRenderOffsetX;
                    Y = LegendOf50.Instance.MapRenderOffsetY + (LegendOf50.Instance.MapHeight / 2) * LegendOf50.Instance.TileSize - LegendOf50.Instance.TileSize;
                    Height = 32;
                    Width = 16;
                    break;
                case DoorwayDirection.Right:
                    X = LegendOf50.Instance.MapRenderOffsetX + (LegendOf50.Instance.MapWidth * LegendOf50.Instance.TileSize) - LegendOf50.Instance.TileSize;
                    Y = LegendOf50.Instance.MapRenderOffsetY + (LegendOf50.Instance.MapHeight / 2 * LegendOf50.Instance.TileSize) - LegendOf50.Instance.TileSize;
                    Height = 32;

                    Width = 16;
                    break;
                case DoorwayDirection.Top:
                    X = LegendOf50.Instance.MapRenderOffsetX + (LegendOf50.Instance.MapWidth / 2 * LegendOf50.Instance.TileSize) - LegendOf50.Instance.TileSize;
                    Y = LegendOf50.Instance.MapRenderOffsetY;
                    Height = 16;
                    Width = 32;
                    break;
                case DoorwayDirection.Bottom:
                    X = LegendOf50.Instance.MapRenderOffsetX + (LegendOf50.Instance.MapWidth / 2 * LegendOf50.Instance.TileSize) - LegendOf50.Instance.TileSize;
                    Y = LegendOf50.Instance.MapRenderOffsetY + (LegendOf50.Instance.MapHeight * LegendOf50.Instance.TileSize) - LegendOf50.Instance.TileSize;
                    Height = 16;
                    Width = 32;
                    break;
            }
        }

        public void Render(int offsetX, int offsetY)
        {
            var texture = LegendOf50.Instance.Textures["tiles"];
            var quads = LegendOf50.Instance.Frames["tiles"];

            // used for shifting the doors when sliding rooms
            X += offsetX;
            Y += offsetY;

            switch (Direction)
            {
                case DoorwayDirection.Left:
                    if (Open)
                    {
                        LegendOf50.Instance.Graphics.Draw(texture, quads[180], X - LegendOf50.Instance.TileSize, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[181], X, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[199], X - LegendOf50.Instance.TileSize, Y + LegendOf50.Instance.TileSize);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[200], X, Y + LegendOf50.Instance.TileSize);
                    }
                    else
                    {
                        LegendOf50.Instance.Graphics.Draw(texture, quads[218], X - LegendOf50.Instance.TileSize, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[219], X, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[237], X - LegendOf50.Instance.TileSize, Y + LegendOf50.Instance.TileSize);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[238], X, Y + LegendOf50.Instance.TileSize);
                    }
                    break;
                case DoorwayDirection.Right:
                    if (Open)
                    {
                        LegendOf50.Instance.Graphics.Draw(texture, quads[171], X, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[172], X + LegendOf50.Instance.TileSize, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[190], X, Y + LegendOf50.Instance.TileSize);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[191], X + LegendOf50.Instance.TileSize, Y + LegendOf50.Instance.TileSize);
                    }
                    else
                    {
                        LegendOf50.Instance.Graphics.Draw(texture, quads[173], X, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[174], X + LegendOf50.Instance.TileSize, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[192], X, Y + LegendOf50.Instance.TileSize);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[193], X + LegendOf50.Instance.TileSize, Y + LegendOf50.Instance.TileSize);
                    }
                    break;
                case DoorwayDirection.Top:
                    if (Open)
                    {
                        LegendOf50.Instance.Graphics.Draw(texture, quads[97], X, Y - LegendOf50.Instance.TileSize);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[98], X + LegendOf50.Instance.TileSize, Y - LegendOf50.Instance.TileSize);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[116], X, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[117], X + LegendOf50.Instance.TileSize, Y);
                    }
                    else
                    {
                        LegendOf50.Instance.Graphics.Draw(texture, quads[133], X, Y - LegendOf50.Instance.TileSize);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[134], X + LegendOf50.Instance.TileSize, Y - LegendOf50.Instance.TileSize);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[152], X, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[153], X + LegendOf50.Instance.TileSize, Y);
                    }
                    break;
                case DoorwayDirection.Bottom:
                    if (Open)
                    {
                        LegendOf50.Instance.Graphics.Draw(texture, quads[140], X, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[141], X + LegendOf50.Instance.TileSize, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[159], X, Y + LegendOf50.Instance.TileSize);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[160], X + LegendOf50.Instance.TileSize, Y + LegendOf50.Instance.TileSize);
                    }
                    else
                    {
                        LegendOf50.Instance.Graphics.Draw(texture, quads[215], X, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[216], X + LegendOf50.Instance.TileSize, Y);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[234], X, Y + LegendOf50.Instance.TileSize);
                        LegendOf50.Instance.Graphics.Draw(texture, quads[235], X + LegendOf50.Instance.TileSize, Y + LegendOf50.Instance.TileSize);
                    }
                    break;
            }

            // revert to original positions
            X -= offsetX;
            Y -= offsetY;
        }
    }
}

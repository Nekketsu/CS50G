namespace LegendOf50Game.World
{
    // Dungeon Class
    public class Dungeon
    {
        public Player Player { get; private set; }

        public List<Room> Rooms { get; private set; }

        public Room CurrentRoom { get; private set; }

        public Room NextRoom { get; private set; }

        public double CameraX { get; private set; }
        public double CameraY { get; private set; }
        public bool Shifting { get; private set; }

        private Dungeon() { }

        public static async Task<Dungeon> Create(Player player)
        {
            var dungeon = new Dungeon();

            dungeon.Player = player;

            dungeon.Rooms = new List<Room>();

            // current room we're operating in
            dungeon.CurrentRoom = await Room.Create(player); ;

            // room we're moving camera to during a shift; become active room afterwards
            dungeon.NextRoom = null;

            // love.graphics.translate values, only when shifting screens
            dungeon.CameraX = 0;
            dungeon.CameraY = 0;
            dungeon.Shifting = false;

            // trigger camera translation and adjustment of rooms whenever the player triggers a shift
            // via a doorway collision, triggered in PlayerWalkState
            LegendOf50.Instance.Event.On("shift-left", () => dungeon.BeginShifting(-LegendOf50.Instance.VirtualWidth, 0));
            LegendOf50.Instance.Event.On("shift-right", () => dungeon.BeginShifting(LegendOf50.Instance.VirtualWidth, 0));
            LegendOf50.Instance.Event.On("shift-up", () => dungeon.BeginShifting(0, -LegendOf50.Instance.VirtualHeight));
            LegendOf50.Instance.Event.On("shift-down", () => dungeon.BeginShifting(0, LegendOf50.Instance.VirtualHeight));

            return dungeon;
        }

        // Prepares for the camera shifting process, kicking off a tween of the camera position.
        private async Task BeginShifting(double shiftX, double shiftY)
        {
            Shifting = true;
            NextRoom = await Room.Create(Player);

            // start all doors in next room as open until we get in
            foreach (var doorway in NextRoom.Doorways)
            {
                doorway.Open = true;
            }

            NextRoom.AdjacentOffsetX = shiftX;
            NextRoom.AdjacentOffsetY = shiftY;

            // tween the player position so they move through the doorway
            var (playerX, playerY) = (Player.X, Player.Y);

            if (shiftX > 0)
            {
                playerX = LegendOf50.Instance.VirtualWidth + (LegendOf50.Instance.MapRenderOffsetX + LegendOf50.Instance.TileSize);
            }
            else if (shiftX < 0)
            {
                playerX = -LegendOf50.Instance.VirtualWidth + (LegendOf50.Instance.MapRenderOffsetX + (LegendOf50.Instance.MapWidth * LegendOf50.Instance.TileSize) - LegendOf50.Instance.TileSize - Player.Width);
            }
            else if (shiftY > 0)
            {
                playerY = LegendOf50.Instance.VirtualHeight + (LegendOf50.Instance.MapRenderOffsetY + Player.Height / 2);
            }
            else
            {
                playerY = -LegendOf50.Instance.VirtualHeight + LegendOf50.Instance.MapRenderOffsetY + (LegendOf50.Instance.MapHeight * LegendOf50.Instance.TileSize) - LegendOf50.Instance.TileSize - Player.Height;
            }

            // tween the camera in whichever direction the new room is in, as well as the player to be
            // at the opposite door in the next room, walking through the wall(which is stenciled)
            var cameraX = CameraX;
            var cameraY = CameraY;
            var x = Player.X;
            var y = Player.Y;
            LegendOf50.Instance.Timer.Tween(TimeSpan.FromSeconds(1), 0, 1, value =>
            {
                CameraX = cameraX * (1 - value) + shiftX * value;
                CameraY = cameraY * (1 - value) + shiftY * value;
                Player.X = x * (1 - value) + playerX * value;
                Player.Y = y * (1 - value) + playerY * value;
            }, async () =>
            {
                FinishShifting();

                // reset player to the correct location in the room
                if (shiftX < 0)
                {
                    Player.X = LegendOf50.Instance.MapRenderOffsetX + (LegendOf50.Instance.MapWidth * LegendOf50.Instance.TileSize) - LegendOf50.Instance.TileSize - Player.Width;
                    Player.Direction = Direction.Left;
                }
                else if (shiftX > 0)
                {
                    Player.X = LegendOf50.Instance.MapRenderOffsetX + LegendOf50.Instance.TileSize;
                    Player.Direction = Direction.Right;
                }
                else if (shiftY < 0)
                {
                    Player.Y = LegendOf50.Instance.MapRenderOffsetY + (LegendOf50.Instance.MapHeight * LegendOf50.Instance.TileSize) - LegendOf50.Instance.TileSize - Player.Height;
                    Player.Direction = Direction.Up;
                }
                else
                {
                    Player.Y = LegendOf50.Instance.MapRenderOffsetY + Player.Height / 2;
                    Player.Direction = Direction.Down;
                }

                // close all doors in the current room
                foreach (var doorway in CurrentRoom.Doorways)
                {
                    doorway.Open = false;
                }

                await LegendOf50.Instance.Sounds["door"].Play();
            });
        }

        // Resets a few variables needed to perform a camera shift and swaps the next and
        // current room.
        private void FinishShifting()
        {
            CameraX = 0;
            CameraY = 0;
            CurrentRoom = NextRoom;
            NextRoom = null;
            CurrentRoom.AdjacentOffsetX = 0;
            CurrentRoom.AdjacentOffsetY = 0;
            Shifting = false;
        }

        public async Task Update(TimeSpan dt)
        {
            // pause updating if we're in the middle of shifting
            if (!Shifting)
            {
                await CurrentRoom.Update(dt);
            }
            else
            {
                // still update the player animation if we're shirting
                Player.CurrentAnimation.Update(dt);
            }
        }

        public async Task Render()
        {
            // translate the camera if we're actively shifting
            if (Shifting)
            {
                LegendOf50.Instance.Graphics.Translate(-(int)CameraX, -(int)CameraY);
            }

            await CurrentRoom.Render();

            if (NextRoom != null)
            {
                await NextRoom.Render();
            }
        }
    }
}

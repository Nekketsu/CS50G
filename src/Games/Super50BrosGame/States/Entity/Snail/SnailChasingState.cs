using GameEngine.States;

namespace Super50BrosGame.States.Entity.Snail
{
    // SnailChasingState Class
    public class SnailChasingState : State
    {
        public TileMap TileMap { get; }
        public Player Player { get; }
        public Super50BrosGame.Snail Snail { get; }
        public Animation Animation { get; private set; }

        public SnailChasingState(TileMap tileMap, Player player, Super50BrosGame.Snail snail)
        {
            TileMap = tileMap;
            Player = player;
            Snail = snail;
        }

        public override Task Enter(Dictionary<string, object> parameters)
        {
            Animation = new Animation(new int[] { 48, 49 }, TimeSpan.FromSeconds(0.5));
            Snail.CurrentAnimation = Animation;

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            Snail.CurrentAnimation.Update(dt);

            // calculate difference between snail and player on X axis
            // and only chase if <= 5 tiles
            var diffX = Math.Abs(Player.X - Snail.X);

            if (diffX > 5 * Super50Bros.Instance.TileSize)
            {
                await Snail.ChangeState("moving");
            }
            else if (Player.X < Snail.X)
            {
                Snail.Direction = Direction.Left;
                Snail.X -= Super50Bros.Instance.SnailMoveSpeed * dt.TotalSeconds;

                // stop the snail if there's a missing tile on the floor to the left or a solid tile directly left
                var tileLeft = TileMap.PointToTile(Snail.X, Snail.Y);
                var tileBottomLeft = TileMap.PointToTile(Snail.X, Snail.Y + Snail.Height);

                if ((tileLeft != null & tileBottomLeft != null) && (tileLeft.Collidable() || !tileBottomLeft.Collidable()))
                {
                    Snail.X += Super50Bros.Instance.SnailMoveSpeed * dt.TotalSeconds;
                }
            }
            else
            {
                Snail.Direction = Direction.Right;
                Snail.X += Super50Bros.Instance.SnailMoveSpeed * dt.TotalSeconds;

                // stop the snail if there's a missing tile on the floor to the right or a solid tile directly right
                var tileRight = TileMap.PointToTile(Snail.X + Snail.Width, Snail.Y);
                var tileBottomRight = TileMap.PointToTile(Snail.X + Snail.Width, Snail.Y + Snail.Height);

                if ((tileRight != null & tileBottomRight != null) && (tileRight.Collidable() || !tileBottomRight.Collidable()))
                {
                    Snail.X -= Super50Bros.Instance.SnailMoveSpeed * dt.TotalSeconds;
                }
            }
        }
    }
}

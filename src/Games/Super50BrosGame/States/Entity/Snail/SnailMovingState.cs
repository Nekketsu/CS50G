using GameEngine.States;

namespace Super50BrosGame.States.Entity.Snail
{
    // SnailMovingState Class
    public class SnailMovingState : State
    {
        public TileMap Tilemap { get; }
        public Player Player { get; }
        public Super50BrosGame.Snail Snail { get; }
        public Animation Animation { get; private set; }

        public Direction MovingDirection { get; private set; }
        public TimeSpan MovingDuration { get; private set; }
        public TimeSpan MovingTimer { get; private set; }

        public SnailMovingState(TileMap tilemap, Player player, Super50BrosGame.Snail snail)
        {
            Tilemap = tilemap;
            Player = player;
            Snail = snail;
        }

        public override Task Enter(Dictionary<string, object> parameters)
        {
            Animation = new Animation(new int[] { 48, 49 }, TimeSpan.FromSeconds(0.5));
            Snail.CurrentAnimation = Animation;

            MovingDirection = Super50Bros.Instance.Random.Next(2) == 0 ? Direction.Left : Direction.Right;
            Snail.Direction = MovingDirection;
            MovingDuration = TimeSpan.FromSeconds(Super50Bros.Instance.Random.Next(5) + 1);
            MovingTimer = TimeSpan.Zero;

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            MovingTimer += dt;
            Snail.CurrentAnimation.Update(dt);

            // reset movement direction and timer if timer is above duration
            if (MovingTimer > MovingDuration)
            {
                // chance to go into idle state randomly
                if (Super50Bros.Instance.Random.Next(4) == 0)
                {
                    await Snail.ChangeState("idle", new System.Collections.Generic.Dictionary<string, object>
                    {
                        // random ammount of time for snail to be idle
                        ["wait"] = TimeSpan.FromSeconds(Super50Bros.Instance.Random.Next(5) + 1)
                    });
                }
                else
                {
                    MovingDirection = Super50Bros.Instance.Random.Next(2) == 0 ? Direction.Left : Direction.Right;
                    Snail.Direction = MovingDirection;
                    MovingDuration = TimeSpan.FromSeconds(Super50Bros.Instance.Random.Next(5));
                    MovingTimer = TimeSpan.Zero;
                }
            }
            else if (Snail.Direction == Direction.Left)
            {
                Snail.X -= Super50Bros.Instance.SnailMoveSpeed * dt.TotalSeconds;

                // stop the snail if there's a missing tile on the floor to the left or a solid tile directly left
                var tileLeft = Tilemap.PointToTile(Snail.X, Snail.Y);
                var tileBottomLeft = Tilemap.PointToTile(Snail.X, Snail.Y + Snail.Height);

                if ((tileLeft != null && tileBottomLeft != null) && (tileLeft.Collidable() || !tileBottomLeft.Collidable()))
                {
                    Snail.X += Super50Bros.Instance.SnailMoveSpeed * dt.TotalSeconds;

                    // reset direction if we hit a wall
                    MovingDirection = Direction.Right;
                    Snail.Direction = MovingDirection;
                    MovingDuration = TimeSpan.FromSeconds(Super50Bros.Instance.Random.Next(5));
                    MovingTimer = TimeSpan.Zero;
                }
            }
            else
            {
                Snail.Direction = Direction.Right;
                Snail.X += Super50Bros.Instance.SnailMoveSpeed * dt.TotalSeconds;

                // stop the snail if there's a missing tile on the floor to the left or a solid tile directly left
                var tileRight = Tilemap.PointToTile(Snail.X + Snail.Width, Snail.Y);
                var tileBottomRight = Tilemap.PointToTile(Snail.X + Snail.Width, Snail.Y + Snail.Height);

                if ((tileRight != null && tileBottomRight != null) && (tileRight.Collidable() || !tileBottomRight.Collidable()))
                {
                    Snail.X -= Super50Bros.Instance.SnailMoveSpeed * dt.TotalSeconds;

                    // reset direction if we hit a wall
                    MovingDirection = Direction.Left;
                    Snail.Direction = MovingDirection;
                    MovingDuration = TimeSpan.FromSeconds(Super50Bros.Instance.Random.Next(5));
                    MovingTimer = TimeSpan.Zero;
                }
            }

            // calculate difference between snail and player on X axis
            // and only chase if <= 5 tiles
            var diffX = Math.Abs(Player.X - Snail.X);

            if (diffX < 5 * Super50Bros.Instance.TileSize)
            {
                await Snail.ChangeState("chasing");
            }
        }
    }
}

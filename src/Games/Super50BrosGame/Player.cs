using GameEngine.States;

namespace Super50BrosGame
{
    // Player Class
    public class Player : Entity
    {
        public int Score { get; set; }

        public Player(double x, double y, int width, int height, string texture, StateMachine stateMachine, TileMap map, GameLevel level) : base(x, y, width, height, texture, stateMachine, map, level)
        {
            Score = 0;
        }

        public void CheckLeftCollisions(TimeSpan dt)
        {
            // check for left two tiles collision
            var tileTopLeft = Map.PointToTile(X + 1, Y + 1);
            var tileBottomLeft = Map.PointToTile(X + 1, Y + Height - 1);

            // place player outside the X bounds on one of the tiles to reset any overlap
            if ((tileTopLeft != null && tileBottomLeft != null) && (tileTopLeft.Collidable() || tileBottomLeft.Collidable()))
            {
                X = tileTopLeft.X * Super50Bros.Instance.TileSize + tileTopLeft.Width - 1;
            }
            else
            {
                Y--;
                var collidedObjects = CheckObjectCollisions();
                Y++;

                // reset X if new collided object
                if (collidedObjects.Any())
                {
                    X += Super50Bros.Instance.PlayerWalkSpeed * dt.TotalSeconds;
                }
            }
        }

        public void CheckRightCollisions(TimeSpan dt)
        {
            // check for right two tiles collision
            var tileTopRight = Map.PointToTile(X + Width - 1, Y + 1);
            var tileBottomRight = Map.PointToTile(X + Width - 1, Y + Height - 1);

            // place player outside the X bounds on one of the tiles to reset any overlap
            if ((tileTopRight != null && tileBottomRight != null) && (tileTopRight.Collidable() || tileBottomRight.Collidable()))
            {
                X = tileTopRight.X * Super50Bros.Instance.TileSize - Width;
            }
            else
            {
                Y--;
                var collidedObjects = CheckObjectCollisions();
                Y++;

                // reset X if new collided object
                if (collidedObjects.Any())
                {
                    X -= Super50Bros.Instance.PlayerWalkSpeed * dt.TotalSeconds;
                }
            }
        }

        public GameObject[] CheckObjectCollisions()
        {
            var collidedObjects = new List<GameObject>();

            foreach (var @object in Level.Objects.ToArray())
            {
                if (@object.Collides(this))
                {
                    if (@object.Solid == true)
                    {
                        collidedObjects.Add(@object);
                    }
                    else if (@object.Consumable == true)
                    {
                        @object.OnConsume(this, @object);
                        Level.Objects.Remove(@object);
                    }
                }
            }

            return collidedObjects.ToArray();
        }
    }
}

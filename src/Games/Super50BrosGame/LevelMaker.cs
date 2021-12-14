namespace Super50BrosGame
{
    // LevelMaker Class
    public class LevelMaker
    {
        public GameLevel Generate(int width, int height)
        {
            var tiles = new Tile[height, width];
            var entities = new List<Entity>();
            var objects = new List<GameObject>();

            // whether we should draw our tiles with toppers
            bool? topper = true;
            var tileset = Super50Bros.Instance.Random.Next(20);
            var topperset = Super50Bros.Instance.Random.Next(20);

            // column by column generation instead of row; sometimes better for platformers
            for (var x = 0; x < width; x++)
            {
                var tileId = Super50Bros.Instance.TileIdEmpty;

                // lay out the empty space
                for (var y = 0; y < 6; y++)
                {
                    tiles[y, x] = new Tile(x, y, tileId, null, tileset, topperset);
                }

                // chance to just be emptiness
                if (Super50Bros.Instance.Random.Next(7) == 0)
                {
                    for (var y = 6; y < height; y++)
                    {
                        tiles[y, x] = new Tile(x, y, tileId, null, tileset, topperset);
                    }
                }
                else
                {
                    tileId = Super50Bros.Instance.TileIdGround;

                    var blockHeight = 4;

                    for (var y = 6; y < height; y++)
                    {
                        tiles[y, x] = new Tile(x, y, tileId, y == 6 ? topper : null, tileset, topperset);
                    }

                    // chance to generate a pillar
                    if (Super50Bros.Instance.Random.Next(8) == 0)
                    {
                        blockHeight = 2;

                        // chance to generate bush on pillar
                        if (Super50Bros.Instance.Random.Next(8) == 0)
                        {
                            objects.Add(new GameObject(
                                "bushes",
                                x * Super50Bros.Instance.TileSize,
                                3 * Super50Bros.Instance.TileSize,
                                16,
                                16,
                                Super50Bros.Instance.BushIds[Super50Bros.Instance.Random.Next(Super50Bros.Instance.BushIds.Length)] + Super50Bros.Instance.Random.Next(4) * 7
                                ));
                        }

                        // pillar tiles
                        tiles[4, x] = new Tile(x, 4, tileId, topper, tileset, topperset);
                        tiles[5, x] = new Tile(x, 5, tileId, null, tileset, topperset);
                        tiles[6, x].Topper = null;
                    }
                    // chance to generate bushes
                    else if (Super50Bros.Instance.Random.Next(8) == 0)
                    {
                        objects.Add(new GameObject(
                            "bushes",
                            x * Super50Bros.Instance.TileSize,
                            5 * Super50Bros.Instance.TileSize,
                            16,
                            16,
                            Super50Bros.Instance.BushIds[Super50Bros.Instance.Random.Next(Super50Bros.Instance.BushIds.Length)] + Super50Bros.Instance.Random.Next(4) * 7,
                            collidable: false));
                    }

                    // chance to spawn a block
                    if (Super50Bros.Instance.Random.Next(10) == 0)
                    {
                        // jump block
                        objects.Add(new GameObject(
                            "jump-blocks",
                            x * Super50Bros.Instance.TileSize,
                            (blockHeight - 1) * Super50Bros.Instance.TileSize,
                            16,
                            16,

                            // make it a random variant
                            Super50Bros.Instance.Random.Next(Super50Bros.Instance.JumpBlocks.Length),
                            collidable: true,
                            hit: false,
                            solid: true,

                            // collision function takes itself
                            onCollide: async obj =>
                            {
                                // spawn a gem if we haven't already
                                if (obj.Hit == false)
                                {
                                    // chance to spawn gem, not guaranteed
                                    if (Super50Bros.Instance.Random.Next(5) == 0)
                                    {
                                        var gem = new GameObject(
                                            "gems",
                                            obj.X,
                                            (blockHeight - 1) * Super50Bros.Instance.TileSize - 4,
                                            16,
                                            16,
                                            Super50Bros.Instance.Random.Next(Super50Bros.Instance.Gems.Length),
                                            collidable: true,
                                            consumable: true,
                                            solid: false,

                                            // gem has its own function to add to the player's score
                                            onConsume: async (player, @object) =>
                                            {
                                                await Super50Bros.Instance.Sounds["pickup"].Play();
                                                player.Score += 100;
                                            }
                                        );

                                        // make the gem move up from the block and play a sound
                                        Console.WriteLine($"Gem: ({gem.X}, {gem.Y}), {(blockHeight - 2) * Super50Bros.Instance.TileSize}");
                                        Super50Bros.Instance.Timer.Tween(TimeSpan.FromSeconds(0.1), gem.Y, (blockHeight - 2) * Super50Bros.Instance.TileSize, value => gem.Y = value);
                                        await Super50Bros.Instance.Sounds["powerup-reveal"].Play();

                                        objects.Add(gem);
                                    }

                                    obj.Hit = true;
                                }

                                await Super50Bros.Instance.Sounds["empty-block"].Play();
                            })
                        );
                    }
                }
            }

            //var map = new TileMap(width, height);
            //map.Tiles = tiles;
            var map = new TileMap(tiles);

            return new GameLevel(entities, objects, map);
        }
    }
}

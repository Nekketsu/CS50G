using GameEngine.States;
using LegendOf50Game.States.Entities;

namespace LegendOf50Game.World
{
    // Room Class
    public class Room
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int[,] Tiles { get; private set; }

        public Entity[] Entities { get; private set; }
        public GameObject[] Objects { get; private set; }
        public Doorway[] Doorways { get; private set; }

        public Player Player { get; private set; }

        public int RenderOffsetX { get; private set; }
        public int RenderOffsetY { get; private set; }

        public double AdjacentOffsetX { get; set; }
        public double AdjacentOffsetY { get; set; }

        private Room() { }

        public static async Task<Room> Create(Player player)
        {
            var room = new Room();

            room.Width = LegendOf50.Instance.MapWidth;
            room.Height = LegendOf50.Instance.MapHeight;

            room.Tiles = room.GenerateWallsAndFloors();

            // entities in the room
            room.Entities = await room.GenerateEntities();

            // game objects in the room
            room.Objects = room.GenerateGameObjects();

            // doorways that lead to other dungeon rooms
            var doorways = new List<Doorway>();
            doorways.Add(new Doorway(DoorwayDirection.Top, false, room));
            doorways.Add(new Doorway(DoorwayDirection.Bottom, false, room));
            doorways.Add(new Doorway(DoorwayDirection.Left, false, room));
            doorways.Add(new Doorway(DoorwayDirection.Right, false, room));

            room.Doorways = doorways.ToArray();

            // reference to player for collisions, etc.
            room.Player = player;

            // used for centering the dungeon rendering
            room.RenderOffsetX = LegendOf50.Instance.MapRenderOffsetX;
            room.RenderOffsetY = LegendOf50.Instance.MapRenderOffsetY;

            // used for drawing when this room is the next room, adjacent to the active
            room.AdjacentOffsetX = 0;
            room.AdjacentOffsetY = 0;

            return room;
        }

        // Randomly creates an assortment of enemies for the player to fight.
        private async Task<Entity[]> GenerateEntities()
        {
            var entities = new List<Entity>();

            var types = new string[] { "skeleton", "slime", "bat", "ghost", "spider" };

            for (var i = 0; i < 10; i++)
            {
                var type = types[LegendOf50.Instance.Random.Next(types.Length)];

                var entity = new Entity(
                    LegendOf50.Instance.EntityDefs[type].Animations,
                    LegendOf50.Instance.EntityDefs[type].WalkSpeed,

                    // ensure X an Y are within bounds of the map
                    LegendOf50.Instance.Random.Next(LegendOf50.Instance.MapRenderOffsetX + LegendOf50.Instance.TileSize,
                        LegendOf50.Instance.VirtualWidth - LegendOf50.Instance.TileSize * 2 - 16 + 1),
                    LegendOf50.Instance.Random.Next(LegendOf50.Instance.MapRenderOffsetY + LegendOf50.Instance.TileSize,
                        LegendOf50.Instance.VirtualHeight - (LegendOf50.Instance.VirtualHeight - LegendOf50.Instance.MapHeight * LegendOf50.Instance.TileSize) + LegendOf50.Instance.MapRenderOffsetY - LegendOf50.Instance.TileSize - 16 + 1),

                    16,
                    16,

                    1
                );

                entity.StateMachine = new StateMachine(new Dictionary<string, State>
                {
                    ["walk"] = new EntityWalkState(entity),
                    ["idle"] = new EntityIdleState(entity)
                });

                await entity.ChangeState("walk");

                entities.Add(entity);
            }

            return entities.ToArray();
        }

        // Randomly creates an assortment of obstacles for the player to navigate around.
        private GameObject[] GenerateGameObjects()
        {
            var objects = new List<GameObject>();

            objects.Add(new GameObject(LegendOf50.Instance.GameObjectDefs["switch"],
                LegendOf50.Instance.Random.Next(LegendOf50.Instance.MapRenderOffsetX + LegendOf50.Instance.TileSize,
                    LegendOf50.Instance.VirtualWidth - LegendOf50.Instance.TileSize * 2 - 16 + 1),
                LegendOf50.Instance.Random.Next(LegendOf50.Instance.MapRenderOffsetY + LegendOf50.Instance.TileSize,
                    LegendOf50.Instance.VirtualHeight - (LegendOf50.Instance.VirtualHeight - LegendOf50.Instance.MapHeight * LegendOf50.Instance.TileSize) + LegendOf50.Instance.MapRenderOffsetY - LegendOf50.Instance.TileSize - 16 + 1)));

            // get a reference to the switch
            var @switch = objects[0];

            // define a function for the switch that will open all doors in the room
            @switch.OnCollide = async () =>
            {
                if (@switch.State == "unpressed")
                {
                    @switch.State = "pressed";
                }

                // open every door in the room if we press the switch
                foreach (var doorway in Doorways)
                {
                    doorway.Open = true;
                }

                await LegendOf50.Instance.Sounds["door"].Play();
            };

            return objects.ToArray();
        }

        // Generates the walls and floors of the room, randomizing the various varieties
        // of said tiles for visual variety.
        private int[,] GenerateWallsAndFloors()
        {
            var tiles = new int[Height, Width];

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var id = LegendOf50.Instance.TileEmpty;

                    if (x == 0 && y == 0)
                    {
                        id = LegendOf50.Instance.TileTopLeftCorner;
                    }
                    else if (x == 0 && y == Height - 1)
                    {
                        id = LegendOf50.Instance.TileBottomLeftCorner;
                    }
                    else if (x == Width - 1 && y == 0)
                    {
                        id = LegendOf50.Instance.TileTopRightCorner;
                    }
                    else if (x == Width - 1 && y == Height - 1)
                    {
                        id = LegendOf50.Instance.TileBottomRightCorner;
                    }

                    // random left-hand walls, right walls, top, bottom, and floors
                    else if (x == 0)
                    {
                        id = LegendOf50.Instance.TileLeftWalls[LegendOf50.Instance.Random.Next(LegendOf50.Instance.TileLeftWalls.Length)];
                    }
                    else if (x == Width - 1)
                    {
                        id = LegendOf50.Instance.TileRightWalls[LegendOf50.Instance.Random.Next(LegendOf50.Instance.TileRightWalls.Length)];
                    }
                    else if (y == 0)
                    {
                        id = LegendOf50.Instance.TileTopWalls[LegendOf50.Instance.Random.Next(LegendOf50.Instance.TileTopWalls.Length)];
                    }
                    else if (y == Height - 1)
                    {
                        id = LegendOf50.Instance.TileBottomWalls[LegendOf50.Instance.Random.Next(LegendOf50.Instance.TileBottomWalls.Length)];
                    }
                    else
                    {
                        id = LegendOf50.Instance.TileFloors[LegendOf50.Instance.Random.Next(LegendOf50.Instance.TileFloors.Length)];
                    }

                    tiles[y, x] = id;
                }
            }

            return tiles;
        }

        public async Task Update(TimeSpan dt)
        {
            // don't update anything if we are sliding to another room (we have offsets)
            if (AdjacentOffsetX != 0 || AdjacentOffsetY != 0) { return; }

            await Player.Update(dt);

            foreach (var entity in Entities)
            {
                // remove entity from the table if health is <= 0
                if (entity.Health <= 0)
                {
                    entity.Dead = true;
                }
                else if (!entity.Dead)
                {
                    await entity.ProcessAI(new Dictionary<string, object> { ["room"] = this }, dt);
                    await entity.Update(dt);
                }

                // collision between the player and entities in the room
                if (!entity.Dead && Player.Collides(entity) && !Player.Invulnerable)
                {
                    await LegendOf50.Instance.Sounds["hit-player"].Play();
                    Player.Damage(1);
                    Player.GoInvulnerable(TimeSpan.FromSeconds(1.5));

                    if (Player.Health == 0)
                    {
                        await LegendOf50.Instance.StateMachine.Change("game-over");
                    }
                }
            }

            foreach (var @object in Objects)
            {
                await @object.Update(dt);

                // trigger collision callback on object
                if (Player.Collides(@object))
                {
                    @object.OnCollide();
                }
            }
        }

        public async Task Render()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var tile = Tiles[y, x];
                    LegendOf50.Instance.Graphics.Draw(LegendOf50.Instance.Textures["tiles"], LegendOf50.Instance.Frames["tiles"][tile],
                        x * LegendOf50.Instance.TileSize + RenderOffsetX + AdjacentOffsetX,
                        y * LegendOf50.Instance.TileSize + RenderOffsetY + AdjacentOffsetY);
                }
            }

            // render doorways; stencils are placed where the arches are after so the player can
            // move through them convincingly
            foreach (var doorway in Doorways)
            {
                doorway.Render((int)AdjacentOffsetX, (int)AdjacentOffsetY);
            }

            foreach (var @object in Objects)
            {
                @object.Render((int)AdjacentOffsetX, (int)AdjacentOffsetY);
            }

            foreach (var entity in Entities)
            {
                if (!entity.Dead)
                {
                    await entity.Render((int)AdjacentOffsetX, (int)AdjacentOffsetY);
                }
            }

            // stencil out the door arches so it looks like the player is going through
            //LegendOf50.Instance.Graphics.Stencil(() =>
            //{
            //    // left
            //    LegendOf50.Instance.Graphics.Rectangle(DrawMode.Fill, -LegendOf50.Instance.TileSize - 6, LegendOf50.Instance.MapRenderOffsetY + (LegendOf50.Instance.MapHeight / 2) * LegendOf50.Instance.TileSize - LegendOf50.Instance.TileSize,
            //        LegendOf50.Instance.TileSize * 2 + 6, LegendOf50.Instance.TileSize);

            //    // right
            //    LegendOf50.Instance.Graphics.Rectangle(DrawMode.Fill, LegendOf50.Instance.MapRenderOffsetX + (LegendOf50.Instance.MapWidth * LegendOf50.Instance.TileSize) - 6,
            //        LegendOf50.Instance.MapRenderOffsetY + (LegendOf50.Instance.MapHeight / 2) * LegendOf50.Instance.TileSize - LegendOf50.Instance.TileSize, LegendOf50.Instance.TileSize * 2 + 6, LegendOf50.Instance.TileSize * 2);

            //    // top
            //    LegendOf50.Instance.Graphics.Rectangle(DrawMode.Fill, LegendOf50.Instance.MapRenderOffsetX + (LegendOf50.Instance.MapWidth / 2) * LegendOf50.Instance.TileSize - LegendOf50.Instance.TileSize,
            //        -LegendOf50.Instance.TileSize - 6, LegendOf50.Instance.TileSize * 2 + 12);

            //    // bottom
            //    LegendOf50.Instance.Graphics.Rectangle(DrawMode.Fill, LegendOf50.Instance.MapRenderOffsetX + (LegendOf50.Instance.MapWidth / 2) * LegendOf50.Instance.TileSize - LegendOf50.Instance.TileSize,
            //        LegendOf50.Instance.VirtualHeight - LegendOf50.Instance.TileSize - 6, LegendOf50.Instance.TileSize * 2 + 12);
            //});

            //LegendOf50.Instance.Graphics.SetStencilTest("less", 1);

            if (Player != null)
            {
                await Player.Render();
            }

            //LegendOf50.Instance.SetStencilTest();
        }
    }
}

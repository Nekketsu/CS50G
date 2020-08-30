using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Super50BrosGame
{
    // GameLevel Class
    public class GameLevel
    {
        public List<Entity> Entities { get; }
        public List<GameObject> Objects { get; }
        public TileMap TileMap { get; }

        public GameLevel(List<Entity> entities, List<GameObject> objects, TileMap tileMap)
        {
            Entities = entities;
            Objects = objects;
            TileMap = tileMap;
        }

        // Remove all nil references from tables in case they've set themselves to nil.
        public void Clear()
        {
            Objects.RemoveAll(o => o == null);
            Entities.RemoveAll(e => e == null);
        }

        public async Task Update(TimeSpan dt)
        {
            TileMap.Update(dt);

            foreach (var @object in Objects)
            {
                await @object.Update(dt);
            }

            foreach (var entity in Entities)
            {
                await entity.Update(dt);
            }
        }

        public void Render()
        {
            TileMap.Render();

            foreach (var @object in Objects)
            {
                @object.Render();
            }

            foreach (var entity in Entities)
            {
                entity.Render();
            }
        }
    }
}

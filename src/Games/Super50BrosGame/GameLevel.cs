﻿namespace Super50BrosGame
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

        public void Render(int camX = 0, int camY = 0)
        {
            TileMap.Render(camX, camY);

            foreach (var @object in Objects)
            {
                @object.Render(camX, camY);
            }

            foreach (var entity in Entities)
            {
                entity.Render(camX, camY);
            }
        }
    }
}

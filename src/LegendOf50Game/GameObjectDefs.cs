using System.Collections.Generic;

namespace LegendOf50Game
{
    // GameObjectsDefs class
    public class GameObjectDefs
    {
        public Dictionary<string, GameObjectDef> Defs { get; }

        public GameObjectDefs()
        {
            Defs = new Dictionary<string, GameObjectDef>
            {
                ["switch"] = new GameObjectDef
                {
                    Type = "switch",
                    Texture = "switches",
                    Frame = 1,
                    Width = 16,
                    Height = 16,
                    Solid = false,
                    DefaultState = "unpressed",
                    States = new Dictionary<string, GameObjectDef>
                    {
                        ["unpressed"] = new GameObjectDef
                        {
                            Frame = 1
                        },
                        ["pressed"] = new GameObjectDef
                        {
                            Frame = 0
                        }
                    }
                },
                ["pot"] = new GameObjectDef
                {

                }
            };
        }
    }
}

using LegendOf50;

namespace LegendOf50Game
{
    public class EntityDefs
    {
        public Dictionary<string, EntityDef> Defs { get; }

        public EntityDefs()
        {
            Defs = new Dictionary<string, EntityDef>
            {
                ["player"] = new EntityDef
                {
                    WalkSpeed = LegendOf50.Instance.PlayerWalkSpeed,
                    Animations = new Dictionary<string, Animation>
                    {
                        ["walk-left"] = new Animation(
                            new int[] { 12, 13, 14, 15 },
                            TimeSpan.FromSeconds(0.15),
                            "character-walk"
                        ),
                        ["walk-right"] = new Animation(
                            new int[] { 4, 5, 6, 7 },
                            TimeSpan.FromSeconds(0.15),
                            "character-walk"
                        ),
                        ["walk-down"] = new Animation(
                            new int[] { 0, 1, 2, 3 },
                            TimeSpan.FromSeconds(0.15),
                            "character-walk"
                        ),
                        ["walk-up"] = new Animation(
                            new int[] { 8, 9, 10, 11 },
                            TimeSpan.FromSeconds(0.15),
                            "character-walk"
                        ),
                        ["idle-left"] = new Animation(
                            new int[] { 12 },
                            TimeSpan.FromSeconds(0.15),
                            "character-walk"
                        ),
                        ["idle-right"] = new Animation(
                            new int[] { 4 },
                            TimeSpan.FromSeconds(1),
                            "character-walk"
                        ),
                        ["idle-down"] = new Animation(
                            new int[] { 0 },
                            TimeSpan.FromSeconds(1),
                            "character-walk"
                        ),
                        ["idle-up"] = new Animation(
                            new int[] { 8 },
                            TimeSpan.FromSeconds(1),
                            "character-walk"
                        ),
                        ["walk-up"] = new Animation(
                            new int[] { 8, 9, 10, 11 },
                            TimeSpan.FromSeconds(1),
                            "character-walk"
                        ),
                        ["sword-left"] = new Animation(
                            new int[] { 12, 13, 14, 15 },
                            TimeSpan.FromSeconds(0.05),
                            "character-swing-sword",
                            false
                        ),
                        ["sword-right"] = new Animation(
                            new int[] { 8, 9, 10, 11 },
                            TimeSpan.FromSeconds(0.05),
                            "character-swing-sword",
                            false
                        ),
                        ["sword-down"] = new Animation(
                            new int[] { 0, 1, 2, 3 },
                            TimeSpan.FromSeconds(0.05),
                            "character-swing-sword",
                            false
                        ),
                        ["sword-up"] = new Animation(
                            new int[] { 4, 5, 6, 7 },
                            TimeSpan.FromSeconds(0.05),
                            "character-swing-sword",
                            false
                        )
                    }
                },
                ["skeleton"] = new EntityDef
                {
                    Texture = "entities",
                    Animations = new Dictionary<string, Animation>
                    {
                        ["walk-left"] = new Animation(
                            new int[] { 21, 22, 23, 22 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-right"] = new Animation(
                            new int[] { 33, 34, 35, 34 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-down"] = new Animation(
                            new int[] { 9, 10, 11, 10 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-up"] = new Animation(
                            new int[] { 45, 46, 47, 46 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["idle-left"] = new Animation(
                            new int[] { 22 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-right"] = new Animation(
                            new int[] { 34 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-down"] = new Animation(
                            new int[] { 10 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-up"] = new Animation(
                            new int[] { 46 },
                            TimeSpan.FromSeconds(1)
                        )
                    }
                },
                ["slime"] = new EntityDef
                {
                    Texture = "entities",
                    Animations = new Dictionary<string, Animation>
                    {
                        ["walk-left"] = new Animation(
                            new int[] { 60, 61, 62, 61 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-right"] = new Animation(
                            new int[] { 72, 73, 74, 73 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-down"] = new Animation(
                            new int[] { 48, 49, 50, 49 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-up"] = new Animation(
                            new int[] { 85, 85, 86, 85 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["idle-left"] = new Animation(
                            new int[] { 61 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-right"] = new Animation(
                            new int[] { 73 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-down"] = new Animation(
                            new int[] { 49 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-up"] = new Animation(
                            new int[] { 85 },
                            TimeSpan.FromSeconds(1)
                        )
                    }
                },
                ["bat"] = new EntityDef
                {
                    Texture = "entities",
                    Animations = new Dictionary<string, Animation>
                    {
                        ["walk-left"] = new Animation(
                            new int[] { 63, 64, 65, 64 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-right"] = new Animation(
                            new int[] { 75, 76, 77, 76 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-down"] = new Animation(
                            new int[] { 51, 52, 53, 52 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-up"] = new Animation(
                            new int[] { 87, 88, 89, 88 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["idle-left"] = new Animation(
                            new int[] { 63, 64, 65, 64 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["idle-right"] = new Animation(
                            new int[] { 75, 76, 77, 76 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["idle-down"] = new Animation(
                            new int[] { 51, 52, 53, 52 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["idle-up"] = new Animation(
                            new int[] { 87, 88, 89, 88 },
                            TimeSpan.FromSeconds(0.2)
                        )
                    }
                },
                ["ghost"] = new EntityDef
                {
                    Texture = "entities",
                    Animations = new Dictionary<string, Animation>
                    {
                        ["walk-left"] = new Animation(
                            new int[] { 66, 67, 68, 67 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-right"] = new Animation(
                            new int[] { 78, 79, 80, 79 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-down"] = new Animation(
                            new int[] { 54, 55, 56, 55 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-up"] = new Animation(
                            new int[] { 90, 91, 92, 91 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["idle-left"] = new Animation(
                            new int[] { 67 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-right"] = new Animation(
                            new int[] { 79 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-down"] = new Animation(
                            new int[] { 55 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-up"] = new Animation(
                            new int[] { 91 },
                            TimeSpan.FromSeconds(1)
                        )
                    }
                },
                ["spider"] = new EntityDef
                {
                    Texture = "entities",
                    Animations = new Dictionary<string, Animation>
                    {
                        ["walk-left"] = new Animation(
                            new int[] { 69, 70, 71, 70 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-right"] = new Animation(
                            new int[] { 81, 82, 83, 82 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-down"] = new Animation(
                            new int[] { 57, 58, 59, 58 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["walk-up"] = new Animation(
                            new int[] { 93, 94, 95, 94 },
                            TimeSpan.FromSeconds(0.2)
                        ),
                        ["idle-left"] = new Animation(
                            new int[] { 70 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-right"] = new Animation(
                            new int[] { 82 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-down"] = new Animation(
                            new int[] { 58 },
                            TimeSpan.FromSeconds(1)
                        ),
                        ["idle-up"] = new Animation(
                            new int[] { 94 },
                            TimeSpan.FromSeconds(1)
                        )
                    }
                }
            };
        }
    }
}

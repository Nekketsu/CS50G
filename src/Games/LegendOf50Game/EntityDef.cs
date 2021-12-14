using LegendOf50;

namespace LegendOf50Game
{
    public class EntityDef
    {
        public int WalkSpeed { get; set; } = 20;
        public Dictionary<string, Animation> Animations { get; set; }
        public string Texture { get; set; }
    }
}

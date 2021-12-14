namespace LegendOf50Game
{
    public class GameObjectDef
    {
        public string Type { get; set; }
        public string Texture { get; set; }
        public int Frame { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Solid { get; set; }
        public string DefaultState { get; set; }
        public Dictionary<string, GameObjectDef> States { get; set; }
    }
}

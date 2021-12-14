namespace LegendOf50Game
{
    // GameObject class
    public class GameObject
    {
        public string Type { get; }
        public string Texture { get; }
        public int Frame { get; }

        public bool Solid { get; }

        public string DefaultState { get; }
        public string State { get; set; }
        public Dictionary<string, GameObjectDef> States { get; }

        public double X { get; }
        public double Y { get; }
        public int Width { get; }
        public int Height { get; }
        public Action OnCollide { get; set; }

        public GameObject(string type, string texture, bool solid, string defaultState, Dictionary<string, GameObjectDef> states, double x, double y, int width, int height, int frame = 1)
        {
            // string identifying this object type
            Type = type;
            Texture = texture;
            Frame = frame;

            // wether it acts as an obstacle or not
            Solid = solid;

            DefaultState = defaultState;
            State = defaultState;
            States = states;

            X = x;
            Y = y;
            Width = width;
            Height = height;

            // default empty collision callback
            OnCollide = () => { };
        }

        public GameObject(GameObjectDef def, double x, double y, int frame = 1)
            : this(def.Type, def.Texture, def.Solid, def.DefaultState, def.States, x, y, def.Width, def.Height, frame)
        {

        }

        public virtual Task Update(TimeSpan dt)
        {
            return Task.CompletedTask;
        }

        public virtual void Render(int adjacentOffsetX, int adjacentOffsetY)
        {
            LegendOf50.Instance.Graphics.Draw(LegendOf50.Instance.Textures[Texture], LegendOf50.Instance.Frames[Texture][States[State]?.Frame ?? Frame],
                X + adjacentOffsetX, Y + adjacentOffsetY);
        }
    }
}

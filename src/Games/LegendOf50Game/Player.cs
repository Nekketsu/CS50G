using LegendOf50;

namespace LegendOf50Game
{
    // Player Class
    public class Player : Entity
    {
        public Player(Dictionary<string, Animation> animations, double x, double y, int width, int height, int walkSpeed, int health, int offsetX = 0, int offsetY = 0) : base(animations, walkSpeed, x, y, width, height, health, offsetX, offsetY)
        {

        }

        public override bool Collides(Entity target)
        {
            var (selfY, selfHeight) = (Y + Height / 2, Height - Height / 2);

            return !(X + Width < target.X || X > target.X + target.Width ||
                     selfY + selfHeight < target.Y || selfY > target.Y + target.Height);
        }
    }
}

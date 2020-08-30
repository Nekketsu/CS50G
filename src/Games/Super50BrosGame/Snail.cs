using GameEngine.States;
using System;

namespace Super50BrosGame
{
    // Snail Class
    public class Snail : Entity
    {
        public Snail(string texture, double x, double y, int width, int height, StateMachine stateMachine, TileMap map, GameLevel level) : base(x, y, width, height, texture, stateMachine, map, level)
        {
        }

        public override void Render()
        {
            Super50Bros.Instance.Graphics.Draw(Super50Bros.Instance.Textures[Texture], Super50Bros.Instance.Frames[Texture][CurrentAnimation.GetCurrentFrame()],
                Math.Floor(X) + 8, Math.Floor(Y) + 8, Direction == Direction.Right, false, 8, 10);
        }
    }
}

using GameEngine.States;

namespace LegendOf50Game.States.Entities
{
    // EntityIdleState Class
    public class EntityIdleState : State
    {
        public Entity Entity { get; }

        public TimeSpan WaitDuration { get; private set; }
        public TimeSpan WaitTimer { get; private set; }

        public EntityIdleState(Entity entity)
        {
            Entity = entity;

            Entity.ChangeAnimation($"idle-{Entity.Direction.ToString().ToLower()}");
        }

        public override Task Enter(Dictionary<string, object> parameters)
        {
            Entity.ChangeAnimation($"idle-{Entity.Direction.ToString().ToLower()}");

            // used for AI waiting
            WaitDuration = TimeSpan.Zero;
            WaitTimer = TimeSpan.Zero;

            return Task.CompletedTask;
        }

        public override async Task ProcessAI(Dictionary<string, object> parameters, TimeSpan dt)
        {
            if (WaitDuration == TimeSpan.Zero)
            {
                WaitDuration = TimeSpan.FromSeconds(LegendOf50.Instance.Random.Next(5) + 1);
            }
            else
            {
                WaitTimer += dt;

                if (WaitTimer > WaitDuration)
                {
                    await Entity.ChangeState("walk");
                }
            }
        }

        public override Task Render()
        {
            var anim = Entity.CurrentAnimation;
            LegendOf50.Instance.Graphics.Draw(LegendOf50.Instance.Textures[anim.Texture], LegendOf50.Instance.Frames[anim.Texture][anim.GetCurrentFrame()],
                Math.Floor(Entity.X - Entity.OffsetX), Math.Floor(Entity.Y - Entity.OffsetY));

            //LegendOf50.Instance.Graphics.SetColor(255, 0, 255, 255);
            //LegendOf50.Instance.Graphics.Rectangle(DrawMode.Line, Entity.X, Entity.Y, Entity.Width, Entity.Health);
            //LegendOf50.Instance.Graphics.SetColor(255, 255, 255, 255);

            return Task.CompletedTask;
        }
    }
}

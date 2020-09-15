using GameEngine.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegendOf50Game.States.Entities.Player
{
    // PlayerIdleState Class
    public class PlayerIdleState : EntityIdleState
    {
        public PlayerIdleState(Entity entity) : base(entity) { }

        public override async Task Enter(Dictionary<string, object> parameters)
        {
            await base.Enter(parameters);

            // render offset for spaced character sprite
            Entity.OffsetY = 5;
            Entity.OffsetX = 0;
        }

        public override async Task Update(TimeSpan dt)
        {
            //await base.Update(dt);

            if (LegendOf50.Instance.Keyboard.IsDown(Key.Left) || LegendOf50.Instance.Keyboard.IsDown(Key.Right)
                || LegendOf50.Instance.Keyboard.IsDown(Key.Up) || LegendOf50.Instance.Keyboard.IsDown(Key.Down))
            {
                await Entity.ChangeState("walk");
            }

            if (LegendOf50.Instance.Keyboard.WasPressed(Key.Space))
            {
                await Entity.ChangeState("swing-sword");
            }
        }
    }
}

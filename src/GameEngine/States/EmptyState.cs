using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameEngine.States
{
    public class EmptyState : State
    {

        public override Task Enter(Dictionary<string, object> parameters)
        {
            return Task.CompletedTask;
        }

        public override Task Exit()
        {
            return Task.CompletedTask;
        }

        public override Task Render()
        {
            return Task.CompletedTask;
        }

        public override Task Update(TimeSpan dt)
        {
            return Task.CompletedTask;
        }
    }
}

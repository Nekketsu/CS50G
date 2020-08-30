using GameEngine.Input;
using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Super50BrosGame.States.Entity
{
    // PlayerIdleState Class
    public class PlayerIdleState : State
    {
        public Player Player { get; }
        public Animation Animation { get; private set; }

        public PlayerIdleState(Player player)
        {
            Player = player;
        }

        public override Task Enter(Dictionary<string, object> parameters)
        {
            Animation = new Animation(new int[] { 0 }, TimeSpan.FromSeconds(1));
            Player.CurrentAnimation = Animation;

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            if (Super50Bros.Instance.Keyboard.IsDown(Key.Left) || Super50Bros.Instance.Keyboard.IsDown(Key.Right))
            {
                await Player.ChangeState("walking");
            }

            if (Super50Bros.Instance.Keyboard.WasPressed(Key.Space))
            {
                await Player.ChangeState("jump");
            }

            // check if we've collided with any entities and die if so
            foreach (var entity in Player.Level.Entities)
            {
                if (entity.Collides(Player))
                {
                    await Super50Bros.Instance.Sounds["death"].Play();
                    await Super50Bros.Instance.StateMachine.Change("start");
                }
            }
        }
    }
}

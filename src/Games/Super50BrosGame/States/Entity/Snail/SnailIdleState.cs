using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Super50BrosGame.States.Entity.Snail
{
    // SnailIdleState Class
    public class SnailIdleState : State
    {
        public TileMap Tilemap { get; }
        public Player Player { get; }
        public Super50BrosGame.Snail Snail { get; }
        public TimeSpan WaitTimer { get; private set; }
        public Animation Animation { get; private set; }
        public TimeSpan WaitPeriod { get; private set; }

        public SnailIdleState(TileMap tilemap, Player player, Super50BrosGame.Snail snail)
        {
            Tilemap = tilemap;
            Player = player;
            Snail = snail;
        }

        public override Task Enter(Dictionary<string, object> parameters)
        {
            WaitTimer = TimeSpan.Zero;
            Animation = new Animation(new int[] { 50 }, TimeSpan.FromSeconds(1));
            Snail.CurrentAnimation = Animation;

            WaitPeriod = (TimeSpan)parameters["wait"];

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            if (WaitTimer < WaitPeriod)
            {
                WaitTimer += dt;
            }
            else
            {
                await Snail.ChangeState("moving");
            }

            // calculate difference between snail and player on X axis
            // and only chase if <= 5 tiles
            var diffX = Math.Abs(Player.X - Snail.X);

            if (diffX < 5 * Super50Bros.Instance.TileSize)
            {
                await Snail.ChangeState("chasing");
            }
        }
    }
}

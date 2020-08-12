using GameEngine;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace CS50G.GameEngine
{
    public class GameLoop : IGameLoop
    {
        private readonly IJSRuntime jsRuntime;
        private float totalTime;

        public event EventHandler<TimeSpan> OnUpdate;

        public GameLoop(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
            totalTime = 0;
        }

        public async Task Initialize()
        {
            await jsRuntime.InvokeVoidAsync("gameLoop.initialize", DotNetObjectReference.Create(this));
        }

        public async Task Run()
        {
            await jsRuntime.InvokeVoidAsync("gameLoop.run");
        }

        [JSInvokable]
        public void Update(float time)
        {
            var elapsedTime = TimeSpan.FromMilliseconds(time - totalTime);
            totalTime = time;

            OnUpdate(this, elapsedTime);
        }
    }
}

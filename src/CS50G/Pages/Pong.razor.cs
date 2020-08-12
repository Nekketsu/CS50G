using CS50G.GameEngine;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace CS50G.Pages
{
    public partial class Pong
    {
        ElementReference canvas;

        int Width { get; set; }
        int Height { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            var gameLoop = new GameLoop(JSRuntime);
            var graphics = new Graphics(JSRuntime, canvas);
            var audio = new Audio(JSRuntime);
            var keyboard = new Keyboard(JSRuntime, canvas);

            var game = new PongGame.Pong(gameLoop, graphics, audio, keyboard);

            Width = game.VirtualWidth;
            Height = game.VirtualHeight;

            StateHasChanged();

            await game.Run();
        }
    }
}

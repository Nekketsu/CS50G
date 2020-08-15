using CS50G.GameEngine;
using GameEngine;
using GameEngine.Input;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CS50G.Pages
{
    public partial class Pong : IDisposable
    {
        ElementReference canvas;

        int Width { get; set; }
        int Height { get; set; }

        const int SuperSampling = 4;

        bool isFullScreen = false;
        Dictionary<string, object> FullScreenAttributes { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            if (Game.Instance != null)
            {
                await Game.Instance.DisposeAsync();
            }

            var gameLoop = new GameLoop(JSRuntime);
            var graphics = new Graphics(JSRuntime, canvas, SuperSampling);
            var audio = new Audio(JSRuntime);
            var keyboard = new Keyboard(JSRuntime, canvas);

            var game = new PongGame.Pong(gameLoop, graphics, audio, keyboard);

            Game.Instance.Keyboard.KeyPressed += Keyboard_KeyPressed;

            Width = game.VirtualWidth;
            Height = game.VirtualHeight;

            StateHasChanged();

            await game.Run();
        }

        private void Keyboard_KeyPressed(object sender, Key e)
        {
            if (e == Key.Escape)
            {
                if (!isFullScreen)
                {
                    isFullScreen = true;
                    FullScreenAttributes = new Dictionary<string, object>
                    {
                        ["style"] = "position: absolute; top: 0; left: 0"
                    };
                }
                else
                {
                    isFullScreen = false;
                    FullScreenAttributes = null;
                }

                StateHasChanged();
            }
        }

        public void Dispose()
        {
            Game.Instance.Keyboard.KeyPressed -= Keyboard_KeyPressed;
        }
    }
}

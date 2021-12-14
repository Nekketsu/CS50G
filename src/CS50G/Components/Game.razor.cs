﻿using CS50G.GameEngine;
using CS50G.Services;
using GameEngine;
using GameEngine.Input;
using Microsoft.AspNetCore.Components;

namespace CS50G.Components
{
    public partial class Game : IDisposable
    {
        [Parameter] public Type Type { get; set; }
        //[Parameter] public string Name { get; set; }

        ElementReference canvas;

        int Width { get; set; }
        int Height { get; set; }

        const int SuperSampling = 4;

        bool isFullScreen = false;
        Dictionary<string, object> FullScreenAttributes { get; set; }

        GameService gameService;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            gameService = new GameService();

            if (gameService.Instance != null)
            {
                await gameService.Instance.DisposeAsync();
            }

            var gameLoop = new GameLoop(JSRuntime);
            var graphics = new Graphics(JSRuntime, canvas, SuperSampling);
            var audio = new Audio(JSRuntime);
            var keyboard = new Keyboard(JSRuntime, canvas);
            var mouse = new Mouse(JSRuntime, canvas);
            IFileSystem fileSystem = new FileSystem(JSRuntime);

            var game = gameService.CreateGame(Type, gameLoop, graphics, audio, keyboard, mouse, fileSystem);
            //var game = gameService.CreateGame(Name, gameLoop, graphics, audio, keyboard, mouse, fileSystem);

            game.Keyboard.KeyPressed += OnKeyPressed;

            Width = game.VirtualWidth;
            Height = game.VirtualHeight;

            StateHasChanged();

            await game.Run();
        }

        private void OnKeyPressed(object sender, Key e)
        {
            if (e == Key.Escape)
            {
                isFullScreen = !isFullScreen;

                StateHasChanged();
            }
        }

        public void Dispose()
        {
            gameService.Instance.Keyboard.KeyPressed -= OnKeyPressed;
        }
    }
}

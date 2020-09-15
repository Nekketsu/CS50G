using CS50G.GameEngine;
using CS50G.Services;
using GameEngine.Input;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CS50G.Pages
{
    public partial class Index : IDisposable
    {
        public string SelectedGame { get; set; }
        public string[] Games { get; set; }

        ElementReference list;
        Keyboard keyboard;

        public Index()
        {
            Games = new string[]
            {
                nameof(Pong),
                nameof(FiftyBird),
                nameof(Breakout),
                nameof(Match3),
                nameof(Super50Bros),
                nameof(LegendOf50)
            };

            SelectedGame = Games.First();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) { return; }

            var gameService = new GameService();

            if (gameService.Instance != null)
            {
                await gameService.Instance.DisposeAsync();
            }

            keyboard = new Keyboard(JSRuntime, list);
            keyboard.KeyPressed += OnKeyPressed;

            await keyboard.Initialize();
        }

        private void OnFocus(string game)
        {
            SelectedGame = game;
        }

        private async void OnKeyPressed(object sender, Key key)
        {
            var selectedIndex = Array.IndexOf(Games, SelectedGame);

            int? offset = null;

            switch (key)
            {
                case Key.Left:
                    if (selectedIndex > 0) { offset = -1; }
                    break;
                case Key.Up:
                    if (selectedIndex >= 2) { offset = -2; }
                    break;
                case Key.Right:
                    if (selectedIndex < Games.Length - 1) { offset = 1; }
                    break;
                case Key.Down:
                    if (selectedIndex < Games.Length - 2) { offset = 2; }
                    break;
                case Key.Enter:
                case Key.Return:
                    // TODO: Use Task.Run to avoid crash on multiple navigations
                    await Task.Run(() => NavigationManager.NavigateTo(SelectedGame));
                    break;
            }

            if (offset.HasValue)
            {
                SelectedGame = Games[selectedIndex + offset.Value];
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            keyboard.KeyPressed -= OnKeyPressed;
        }
    }
}

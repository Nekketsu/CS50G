using CS50G.Services;
using System.Threading.Tasks;

namespace CS50G.Pages
{
    public partial class Index
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) { return; }

            var gameService = new GameService();

            if (gameService.Instance != null)
            {
                await gameService.Instance.DisposeAsync();
            }
        }
    }
}

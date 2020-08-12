using GameEngine.Audio;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace CS50G.GameEngine
{
    public class Audio : IAudio
    {
        private readonly IJSRuntime jsRuntime;

        public Audio(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task<ISource> NewSource(string fileName)
        {
            await jsRuntime.InvokeVoidAsync("audio.loadSound", fileName.ToAssetUri());

            var source = new Source(jsRuntime, fileName);
            return source;
        }
    }
}

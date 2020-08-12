using GameEngine.Audio;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace CS50G.GameEngine
{
    public class Source : ISource
    {
        private readonly IJSRuntime jsRuntime;
        private readonly string fileName;

        public Source(IJSRuntime jsRuntime, string fileName)
        {
            this.jsRuntime = jsRuntime;
            this.fileName = fileName;
        }

        public async Task Play()
        {
            await jsRuntime.InvokeVoidAsync("audio.play", fileName.ToAssetUri());
        }
    }
}

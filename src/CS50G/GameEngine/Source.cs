using GameEngine.Audio;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace CS50G.GameEngine
{
    public class Source : ISource
    {
        private readonly IJSRuntime jsRuntime;
        private readonly string fileName;

        public bool Looping { get; set; }

        public Source(IJSRuntime jsRuntime, string fileName)
        {
            this.jsRuntime = jsRuntime;
            this.fileName = fileName;

            Looping = false;
        }

        public async Task Play()
        {
            await jsRuntime.InvokeVoidAsync("audio.play", fileName.ToAssetUri(), Looping);
        }

        public async Task Pause()
        {
            await jsRuntime.InvokeVoidAsync("audio.pause", fileName.ToAssetUri());
        }


        public async Task Stop()
        {
            await jsRuntime.InvokeVoidAsync("audio.stop", fileName.ToAssetUri());
        }
    }
}

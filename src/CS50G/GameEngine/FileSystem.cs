using GameEngine;
using Microsoft.JSInterop;

namespace CS50G.GameEngine
{
    public class FileSystem : IFileSystem
    {
        string identity;

        private readonly IJSRuntime jsRuntime;

        public FileSystem(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;

            identity = null;
        }

        public void SetIdentity(string identity)
        {
            this.identity = identity;
        }
        public async Task<bool> Exists(string name)
        {
            var key = GetKey(name);

            var exists = await jsRuntime.InvokeAsync<bool>("fileSystem.exists", key);

            return exists;
        }

        public async Task Write(string name, string value)
        {
            var key = GetKey(name);

            await jsRuntime.InvokeVoidAsync("fileSystem.write", key, value);
        }

        public async Task<string[]> Lines(string name)
        {
            var key = GetKey(name);

            var value = await jsRuntime.InvokeAsync<string>("fileSystem.read", key);

            var lines = value.Split(Environment.NewLine);

            return lines;
        }

        private string GetKey(string name)
        {
            if (!string.IsNullOrWhiteSpace(identity))
            {
                return $"{identity}/{name}";
            }

            return name;
        }
    }
}

using GameEngine;
using GameEngine.Graphics;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CS50G.GameEngine
{
    public class Graphics : IGraphics
    {
        private readonly IJSRuntime jsRuntime;
        private readonly ElementReference canvas;
        private readonly int superSampling;

        private TaskCompletionSource<Image> newImageTask;

        List<string> commands;

        public Graphics(IJSRuntime jsRuntime, ElementReference canvas, int superSampling = 1)
        {
            this.jsRuntime = jsRuntime;
            this.canvas = canvas;
            this.superSampling = superSampling;
            commands = new List<string>();
        }

        public async Task Initialize()
        {
            await jsRuntime.InvokeVoidAsync("graphics.initialize", DotNetObjectReference.Create(this), canvas, superSampling); ;
        }

        public async Task<Font> NewFont(string fileName, int size)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            await jsRuntime.InvokeVoidAsync("graphics.newFont", name, fileName.ToAssetUri(), size);

            var font = new Font(name, size);
            return font;
        }

        public void Begin()
        {
            commands.Clear();
        }

        public async Task Apply()
        {
            await jsRuntime.InvokeVoidAsync("graphics.apply", string.Join(Environment.NewLine, commands));
        }

        public void Clear(int red, int green, int blue)
        {
            SetColor(red, green, blue);
            Rectangle(DrawMode.Fill, 0, 0, Game.Instance.VirtualWidth, Game.Instance.VirtualHeight);
        }

        public void Clear(int red, int green, int blue, int alpha)
        {
            SetColor(red, green, blue, alpha);
            Rectangle(DrawMode.Fill, 0, 0, Game.Instance.VirtualWidth, Game.Instance.VirtualHeight);
        }

        public void Print(string text, int x, int y)
        {
            Print(text, x, y, x, Alignment.Left);
        }

        public void Print(string text, int x, int y, int limit, Alignment alignment)
        {
            var positionX = alignment switch
            {
                Alignment.Left => x,
                Alignment.Center => (x + limit) / 2,
                Alignment.Right => limit,
                _ => throw new NotImplementedException()
            };

            commands.Add($"context.textAlign = '{alignment.ToString().ToLower()}';");
            //commands.Add($"graphics.wrapText(context, '{text}', {positionX}, {y}, {limit});");
            commands.Add($"context.fillText('{text}', {positionX}, {y});");
        }

        public void Rectangle(DrawMode mode, int x, int y, int width, int height)
        {
            commands.Add("context.beginPath();");
            commands.Add($"context.rect({x}, {y}, {width}, {height});");
            var drawMethod = mode == DrawMode.Fill ? "fill" : "stroke";
            commands.Add($"context.{drawMethod}();");
        }

        public void Rectangle(DrawMode mode, double x, double y, double width, double height)
        {
            Rectangle(mode, (int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height));
        }

        public void SetColor(int red, int green, int blue)
        {
            commands.Add($"context.fillStyle = 'rgb({red}, {green}, {blue})';");
        }

        public void SetColor(int red, int green, int blue, int alpha)
        {
            commands.Add($"context.fillStyle = 'rgba({red}, {green}, {blue}, {alpha})';");
        }

        public void SetFont(Font font)
        {
            commands.Add($"context.font = '{font.Size}px {font.Name}'");
        }

        //public async Task<Image> NewImage(string fileName)
        //{
        //    var image = await jsRuntime.InvokeAsync<Image>("graphics.newImage", fileName.ToAssetUri());

        //    return image;
        //}

        public async Task<Image> NewImage(string fileName)
        {
            newImageTask = new TaskCompletionSource<Image>();
            await jsRuntime.InvokeVoidAsync("graphics.newImage", fileName.ToAssetUri());

            return await newImageTask.Task;
        }

        [JSInvokable]
        public void NewImageCompleted(Image image)
        {
            newImageTask.SetResult(image);
        }

        public void Draw(Image image, double x, double y)
        {
            commands.Add($"context.drawImage(graphics.images['{image.Name}'], {Math.Round(x)}, {Math.Round(y)}, {image.Width}, {image.Height});");
        }

        public void Draw(Image image, double x, double y, bool flipHorizontally, bool flipVertically)
        {
            if (flipHorizontally || flipVertically)
            {
                var scaleCommand = $"context.scale({(flipHorizontally ? -1 : 1)}, {(flipVertically ? -1 : 1)});";

                commands.Add(scaleCommand);
                Draw(image, x, -y);
                commands.Add(scaleCommand);
            }
            else
            {
                Draw(image, x, y);
            }
        }
    }
}

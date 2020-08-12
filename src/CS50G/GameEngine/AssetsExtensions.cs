using GameEngine;

namespace CS50G.GameEngine
{
    public static class AssetsExtensions
    {
        public static string ToAssetUri(this string fileName)
        {
            var uri = $"Assets/{Game.Instance.GameName}/{fileName}";

            return uri;
        }
    }
}

namespace GameEngine
{
    public interface IFileSystem
    {
        void SetIdentity(string identity);
        Task<bool> Exists(string key);
        Task Write(string key, string value);
        Task<string[]> Lines(string key);
    }
}

namespace GameEngine.Audio
{
    public interface IAudio
    {
        Task<ISource> NewSource(string fileName);
    }
}

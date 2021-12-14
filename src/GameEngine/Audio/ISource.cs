namespace GameEngine.Audio
{
    public interface ISource
    {
        bool Looping { get; set; }
        double Volume { get; set; }
        Task Play();
        Task Stop();
    }
}

using System.Threading.Tasks;

namespace GameEngine.Audio
{
    public interface ISource
    {
        bool Looping { get; set; }
        Task Play();
        Task Stop();
    }
}

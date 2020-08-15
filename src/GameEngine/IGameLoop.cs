using System;
using System.Threading.Tasks;

namespace GameEngine
{
    public interface IGameLoop
    {
        event EventHandler<TimeSpan> OnUpdate;

        Task Initialize();
        Task Run();
        Task Stop();
    }
}

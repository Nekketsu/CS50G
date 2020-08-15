using System;
using System.Threading.Tasks;

namespace GameEngine
{
    public static class TaskExtensions
    {
        public async static void Await(this Task task, Action completedCallback = null, Action<Exception> errorCallback = null)
        {
            try
            {
                await task;
                completedCallback?.Invoke();
            }
            catch (Exception exception)
            {
                errorCallback?.Invoke(exception);
            }
        }

        public async static void Await<T>(this Task<T> task, Action<T> completedCallback = null, Action<Exception> errorCallback = null)
        {
            try
            {
                var result = await task;
                completedCallback?.Invoke(result);
            }
            catch (Exception exception)
            {
                errorCallback?.Invoke(exception);
            }
        }
    }
}

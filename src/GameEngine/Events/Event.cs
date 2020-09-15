using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameEngine.Events
{
    public class Event
    {
        Dictionary<string, List<Func<Task>>> eventActions;

        public Event()
        {
            eventActions = new Dictionary<string, List<Func<Task>>>();
        }

        public void On(string eventName, Func<Task> action)
        {
            if (!eventActions.TryGetValue(eventName, out var actions))
            {
                actions = new List<Func<Task>>();
                eventActions.Add(eventName, actions);
            }

            actions.Add(action);
        }

        public async Task Dispatch(string eventName)
        {
            if (eventActions.TryGetValue(eventName, out var actions))
            {
                foreach (var action in actions)
                {
                    await action();
                }
            }
        }
    }
}

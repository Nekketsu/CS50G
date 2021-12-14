namespace GameEngine.States
{
    // BaseState Class

    // Used as the base class for all of our states, so we don't have to
    // define empty methods in each of them.StateMachine requires each
    // State have a set of four "interface" methods that it can reliably call,
    // so by inheriting from this base state, our State classes will all have

    // at least empty versions of these methods even if we don't define them

    // ourselves in the actual classes.
    public abstract class State
    {
        public virtual Task Enter(Dictionary<string, object> parameters)
        {
            return Task.CompletedTask;
        }

        public virtual Task Exit()
        {
            return Task.CompletedTask;
        }

        public virtual Task Update(TimeSpan dt)
        {
            return Task.CompletedTask;
        }

        public virtual Task Render()
        {
            return Task.CompletedTask;
        }

        public virtual Task ProcessAI(Dictionary<string, object> parameters, TimeSpan dt)
        {
            return Task.CompletedTask;
        }
    }
}

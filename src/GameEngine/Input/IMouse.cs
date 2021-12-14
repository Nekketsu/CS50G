﻿namespace GameEngine.Input
{
    public interface IMouse
    {
        event EventHandler<MouseButtonArgs> MousePressed;

        Task Initialize();
        Task DisposeAsync();
    }
}

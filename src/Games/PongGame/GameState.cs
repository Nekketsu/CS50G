namespace PongGame
{
    public enum GameState
    {
        /// <summary>
        /// The beginning of the game, before first serve
        /// </summary>
        Start,
        /// <summary>
        /// Waiting on a key press to serve the ball
        /// </summary>
        Serve,
        /// <summary>
        /// The ball is in play, bouncing between paddles
        /// </summary>
        Play,
        /// <summary>
        /// The game is over, with a victor, ready for restart
        /// </summary>
        Done
    }
}

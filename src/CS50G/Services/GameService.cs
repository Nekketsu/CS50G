using GameEngine;
using GameEngine.Audio;
using GameEngine.Graphics;
using GameEngine.Input;

namespace CS50G.Services
{
    public class GameService
    {
        public Game CreateGame(string gameName, IGameLoop gameLoop, IGraphics graphics, IAudio audio, IKeyboard keyboard, IMouse mouse, IFileSystem fileSystem)
        {
            var gameType = typeof(Game);

            var game = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .SingleOrDefault(type => type.Name == gameName
                                      && type.IsSubclassOf(gameType)
                                      && !type.IsAbstract);

            var constructors = game.GetConstructors();
            var constructor = constructors.FirstOrDefault();
            var parameters = constructor.GetParameters();

            var arguments = new List<object>();
            arguments.Add(gameLoop);
            arguments.Add(graphics);
            arguments.Add(audio);
            arguments.Add(keyboard);
            if (parameters.Any(p => p.Name == nameof(mouse))) { arguments.Add(mouse); }
            if (parameters.Any(p => p.Name == nameof(fileSystem))) { arguments.Add(fileSystem); }

            var gameInstance = (Game)Activator.CreateInstance(game, arguments.ToArray());

            return gameInstance;
        }

        public Game CreateGame(Type gameType, IGameLoop gameLoop, IGraphics graphics, IAudio audio, IKeyboard keyboard, IMouse mouse, IFileSystem fileSystem)
        {
            var constructor = gameType.GetConstructors().Single();
            var parameters = constructor.GetParameters();

            var arguments = new List<object>();
            arguments.Add(gameLoop);
            arguments.Add(graphics);
            arguments.Add(audio);
            arguments.Add(keyboard);
            if (parameters.Any(p => p.Name == nameof(mouse))) { arguments.Add(mouse); }
            if (parameters.Any(p => p.Name == nameof(fileSystem))) { arguments.Add(fileSystem); }

            var gameInstance = (Game)Activator.CreateInstance(gameType, arguments.ToArray());

            return gameInstance;
        }

        public Game Instance => Game.Instance;
    }
}

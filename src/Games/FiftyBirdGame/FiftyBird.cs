using FiftyBirdGame.States;
using GameEngine;
using GameEngine.Audio;
using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiftyBirdGame
{
    // Flappy Bird Remake

    // A mobile game by Dong Nguyen that went viral in 2013, utilizing a very simple 
    // but effective gameplay mechanic of avoiding pipes indefinitely by just tapping 
    // the screen, making the player's bird avatar flap its wings and move upwards slightly. 
    // A variant of popular games like "Helicopter Game" that floated around the internet
    // for years prior. Illustrates some of the most basic procedural generation of game
    // levels possible as by having pipes stick out of the ground by varying amounts, acting
    // as an infinitely generated obstacle course for the player.
    public class FiftyBird : Game
    {
        public static new FiftyBird Instance { get; private set; }

        public new StatefulKeyboard Keyboard { get; }
        public new StatefulMouse Mouse { get; }

        public FiftyBird(IGameLoop gameLoop, IGraphics graphics, IAudio audio, IKeyboard keyboard, IMouse mouse) : base(nameof(FiftyBird), gameLoop, graphics, audio, keyboard, mouse)
        {
            Keyboard = new StatefulKeyboard(keyboard);
            Mouse = new StatefulMouse(mouse);

            // initialize state machine with all state - returning functions
            StateMachine = new StateMachine(new Dictionary<string, State>
            {
                ["title"] = new TitleScreenState(),
                ["countdown"] = new CountdownState(),
                ["play"] = new PlayState(),
                ["score"] = new ScoreState()
            });

            Instance = this;
        }

        public override int VirtualWidth => 512;
        public override int VirtualHeight => 288;

        const int BackgroundScrollSpeed = 30;
        const int GroundScrollSpeed = 60;

        const int BackgroundLoopingPoint = 413;

        Image background;
        double backgroundScroll;

        Image ground;
        double groundScroll;

        public Font SmallFont { get; private set; }
        public Font MediumFont { get; private set; }
        public Font FlappyFont { get; private set; }
        public Font HugeFont { get; private set; }

        public StateMachine StateMachine { get; private set; }

        public override async Task Load()
        {
            background = await Graphics.NewImage("background.png");
            backgroundScroll = 0;

            ground = await Graphics.NewImage("ground.png");
            groundScroll = 0;

            // initialize our nice-looking retro text fonts
            SmallFont = await Graphics.NewFont("font.ttf", 8);
            MediumFont = await Graphics.NewFont("flappy.ttf", 14);
            FlappyFont = await Graphics.NewFont("flappy.ttf", 28);
            HugeFont = await Graphics.NewFont("flappy.ttf", 56);
            Graphics.SetFont(FlappyFont);

            // initialize our talbe of sounds
            Sounds.Add("jump", await Audio.NewSource("jump.wav"));
            Sounds.Add("explosion", await Audio.NewSource("explosion.wav"));
            Sounds.Add("hurt", await Audio.NewSource("hurt.wav"));
            Sounds.Add("score", await Audio.NewSource("score.wav"));

            // https://freesound.org/people/xsgianni/sounds/388079/
            Sounds.Add("music", await Audio.NewSource("marios_way.mp3"));

            // kick off music
            Sounds["music"].Looping = true;
            await Sounds["music"].Play();

            await StateMachine.Change("title");
        }

        public override void KeyPressed(Key key)
        {
            if (key == Key.Escape)
            {
                // Quit
            }
        }

        // LÖVE2D callback fired each time a mouse button is pressed; gives us the
        // X and Y of the mouse, as well as the button in question.
        public override void MousePressed(int x, int y, MouseButton button)
        {

        }

        public override async Task Update(TimeSpan dt)
        {
            // scroll our background and ground, looping back to 0 after a certain amount
            backgroundScroll = (backgroundScroll + BackgroundScrollSpeed * dt.TotalSeconds) % BackgroundLoopingPoint;
            groundScroll = (groundScroll + GroundScrollSpeed * dt.TotalSeconds) % VirtualWidth;

            await StateMachine.Update(dt);

            Keyboard.Update();
            Mouse.Update();
        }

        public override async Task Draw()
        {
            Graphics.Begin();

            // begin drawing with push, in our virtual resolution
            Graphics.Clear(40, 45, 52);

            Graphics.SetColor(255, 255, 255);


            Graphics.Draw(background, -backgroundScroll, 0);
            await StateMachine.Render();
            Graphics.Draw(ground, -groundScroll, VirtualHeight - 16);

            await Graphics.Apply();
        }
    }
}

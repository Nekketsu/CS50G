using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.States;
using Match3Game.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Match3Game.States
{
    // StartState Class

    // Represents the state the game is in when we've just started; should
    // simply display "Match-3" in large text, as well as a message to press
    // Enter to begin.
    public class StartState : State
    {
        List<Quad> positions;

        int currentMenuItem;

        Color[] colors;

        KeyValuePair<char, int>[] letterTable;

        Every colorTimer;

        int transitionAlpha;

        bool pauseInput;


        public override Task Enter(Dictionary<string, object> parameters)
        {
            positions = new List<Quad>();

            // currently selected menu item
            currentMenuItem = 0;

            // colors we'll use to change the title text
            colors = new Color[]
            {
                new Color(217, 87, 99, 255),
                new Color(95, 205, 228, 255),
                new Color(251, 242, 54, 255),
                new Color(118, 66, 138, 255),
                new Color(153, 229, 80, 255),
                new Color(223, 113, 38, 255)
            };

            // letters of MATCH 3 and their spacing relative to the center
            letterTable = new[]
            {
                new KeyValuePair<char, int>('M', -108),
                new KeyValuePair<char, int>('A', -64),
                new KeyValuePair<char, int>('T', -28),
                new KeyValuePair<char, int>('C', 2),
                new KeyValuePair<char, int>('H', 40),
                new KeyValuePair<char, int>('3', 112)
            };

            // time for a color change if it's been half a second
            colorTimer = Match3.Instance.Timer.Every(TimeSpan.FromSeconds(0.075), () =>
            {
                var color = colors[6 - 1];

                for (var i = 6 - 1; i > 0; i--)
                {
                    colors[i] = colors[i - 1];
                }

                colors[0] = color;
            });

            // generate full table of tiles just for display
            for (var i = 0; i < 64; i++)
            {
                positions.Add(Match3.Instance.Frames["tiles"][Match3.Instance.Random.Next(18)][Match3.Instance.Random.Next(6)]);
            }

            // used to animate our full-screen transition rect
            transitionAlpha = 0;

            // if we've selected an option, we need to pause input while we animate out
            pauseInput = false;

            return Task.CompletedTask;
        }

        public override async Task Update(TimeSpan dt)
        {
            if (Match3.Instance.Keyboard.WasPressed(Key.Escape))
            {
                // Quit
            }

            // as long as can still input, i.e., we're not in a transition...
            if (!pauseInput)
            {
                // change menu selection
                if (Match3.Instance.Keyboard.WasPressed(Key.Up) || Match3.Instance.Keyboard.WasPressed(Key.Down))
                {
                    currentMenuItem = currentMenuItem == 0 ? 1 : 0;
                    await Match3.Instance.Sounds["select"].Play();
                }

                // switch to another state via one of the menu options
                if (Match3.Instance.Keyboard.WasPressed(Key.Enter) || Match3.Instance.Keyboard.WasPressed(Key.Return))
                {
                    if (currentMenuItem == 0)
                    {
                        // tween, using Timer, the transition rect's alpha to 255, then
                        // transition to the BeginGame state after the animation is over
                        Match3.Instance.Timer.Tween(TimeSpan.FromSeconds(1), transitionAlpha, 255, value => transitionAlpha = (int)value, async () =>
                        {
                            Match3.Instance.Timer.Clear();
                            await Match3.Instance.StateMachine.Change("begin-game", new Dictionary<string, object>
                            {
                                ["level"] = 1
                            });
                        });

                        // remove color timer from Timer
                        colorTimer.Remove();

                        // turn off input during transition
                        pauseInput = true;
                    }
                    else
                    {
                        // Quit
                    }
                }
            }

            // update our Timer, which will be used for our fade transitions
            // Timer.Update(dt);
        }

        public override Task Render()
        {
            // render all tiles and their drop shadows
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    // render shadow first
                    Match3.Instance.Graphics.SetColor(0, 0, 0, 255);
                    Match3.Instance.Graphics.Draw(Match3.Instance.Textures["main"], positions[y * (x + 1) + x], x * 32 + 128 + 3, y * 32 + 16 + 3);
                    // TODO: workaround to tint an image
                    Match3.Instance.Graphics.Rectangle(DrawMode.Fill, x * 32 + 128 + 3, y * 32 + 16 + 3, 32, 32, 4);

                    // render tile
                    Match3.Instance.Graphics.SetColor(255, 255, 255, 255);
                    Match3.Instance.Graphics.Draw(Match3.Instance.Textures["main"], positions[y * (x + 1) + x], x * 32 + 128, y * 32 + 16);
                }
            }

            // keep the background and tiles a little darker than normal
            Match3.Instance.Graphics.SetColor(0, 0, 0, 128);
            Match3.Instance.Graphics.Rectangle(DrawMode.Fill, 0, 0, Match3.Instance.VirtualWidth, Match3.Instance.VirtualHeight);

            DrawMatch3Text(-60);
            DrawOptions(12);

            // draw our transition rect; is normally fully transparent, unless we're moving to a new state
            Match3.Instance.Graphics.SetColor(255, 255, 255, transitionAlpha);
            Match3.Instance.Graphics.Rectangle(DrawMode.Fill, 0, 0, Match3.Instance.VirtualWidth, Match3.Instance.VirtualHeight);

            return Task.CompletedTask;
        }

        // Draw the centered MATCH-3 text with background rect, placed along the Y
        // axis as needed, relative to the center.
        private void DrawMatch3Text(int y)
        {
            // draw semi-transparent rect behind MATCH 3
            Match3.Instance.Graphics.SetColor(255, 255, 255, 128);
            Match3.Instance.Graphics.Rectangle(DrawMode.Fill, Match3.Instance.VirtualWidth / 2 - 76, Match3.Instance.VirtualHeight / 2 + y - 11, 150, 58, 6);

            // draw MATCH 3 text shadows
            Match3.Instance.Graphics.SetFont(Match3.Instance.Fonts["large"]);
            DrawTextShadow("MATCH 3", Match3.Instance.VirtualWidth / 2 + y);

            // print MATCH 3 letters in their corresponding current colors
            for (var i = 0; i < 6; i++)
            {
                var color = colors[i];
                Match3.Instance.Graphics.SetColor(color.R, color.G, color.B, color.A);
                Match3.Instance.Graphics.Print($"{letterTable[i].Key}", 0, Match3.Instance.VirtualHeight / 2 + y, Match3.Instance.VirtualWidth + letterTable[i].Value, Alignment.Center);
            }
        }

        // Draws "Start" and "Quit Game" text over semi-transparent rectangles.
        private void DrawOptions(int y)
        {
            // draw rect behind start and quit game text
            Match3.Instance.Graphics.SetColor(255, 255, 255, 128);
            Match3.Instance.Graphics.Rectangle(DrawMode.Fill, Match3.Instance.VirtualWidth / 2 - 76, Match3.Instance.VirtualHeight / 2 + y, 150, 58, 6);

            // draw Start text
            Match3.Instance.Graphics.SetFont(Match3.Instance.Fonts["medium"]);
            DrawTextShadow("Start", Match3.Instance.VirtualHeight / 2 + y + 8);

            if (currentMenuItem == 0)
            {
                Match3.Instance.Graphics.SetColor(99, 155, 255, 255);
            }
            else
            {
                Match3.Instance.Graphics.SetColor(48, 96, 130, 255);
            }

            Match3.Instance.Graphics.Print("Start", 0, Match3.Instance.VirtualHeight / 2 + y + 8, Match3.Instance.VirtualWidth, Alignment.Center);

            // draw Quit Game text
            Match3.Instance.Graphics.SetFont(Match3.Instance.Fonts["medium"]);
            DrawTextShadow("Quit Game", Match3.Instance.VirtualHeight / 2 + y + 33);

            if (currentMenuItem == 1)
            {
                Match3.Instance.Graphics.SetColor(99, 155, 255, 255);
            }
            else
            {
                Match3.Instance.Graphics.SetColor(48, 96, 130, 255);
            }

            Match3.Instance.Graphics.Print("Quit Game", 0, Match3.Instance.VirtualHeight / 2 + y + 33, Match3.Instance.VirtualWidth, Alignment.Center);
        }

        // Helper function for drawing just text backgrounds; draws several layers of the same text, in
        // black, over top of one another for a thicker shadow.
        private void DrawTextShadow(string text, int y)
        {
            Match3.Instance.Graphics.SetColor(34, 32, 52, 255);
            Match3.Instance.Graphics.Print(text, 2, y + 1, Match3.Instance.VirtualWidth, Alignment.Center);
            Match3.Instance.Graphics.Print(text, 1, y + 1, Match3.Instance.VirtualWidth, Alignment.Center);
            Match3.Instance.Graphics.Print(text, 0, y + 1, Match3.Instance.VirtualWidth, Alignment.Center);
            Match3.Instance.Graphics.Print(text, 1, y + 2, Match3.Instance.VirtualWidth, Alignment.Center);
        }
    }
}

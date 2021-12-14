using GameEngine.Graphics;

namespace BreakoutGame
{
    // Brick Class

    // Represents a brick in the world space that the ball can collide with;
    // differently colored bricks have different point values. On collision,
    // the ball will bounce away depending on the angle of collision. When all
    // bricks are cleared in the current map, the player should be taken to a new
    // layout of bricks.
    public class Brick
    {
        static readonly Color[] paletteColors = new Color[]
        {
            new Color(99, 155, 255), // blue
            new Color(106, 190, 47), // green
            new Color(217, 87, 99), // red
            new Color(215, 123, 186), // purple
            new Color(251, 242, 54) // gold
        };

        public int Tier { get; set; }
        public int Color { get; set; }

        public double X { get; }
        public double Y { get; }

        public int Width { get; set; }
        public int Height { get; set; }

        public bool InPlay { get; private set; }

        //ParticleSystem particleSystem;
        public Brick(double x, double y)
        {
            // used for coloring and score calculation
            Tier = 0;
            Color = 0;

            X = x;
            Y = y;
            Width = 32;
            Height = 16;

            // used to determine whether this brick should be rendered
            InPlay = true;

            //// particle system belonging to the brick, emitted on hit
            //particleSystem = Breakout.Instance.Graphics.NewParticleSystem(Breakout.Instance.Textures["particle"], 64);

            //// various behavior - determining functions for the particle system
            //// https://love2d.org/wiki/ParticleSystem


            //// lasts between 0.5 - 1 seconds seconds
            //particleSystem.SetParticleLifetime(0.5, 1);

            //// give it an acceleration of anywhere between X1, Y1 and X2, Y2(0, 0) and(80, 80) here
            //// gives generally downward
            //particleSystem.SetLinearAcceleration(-15, 0, 15, 80);

            //// spread of particles; normal looks more natural than uniform
            //particleSystem.SetAreaSpread("normal", 10, 10);
        }

        // Triggers a hit on the brick, taking it out of play if at 0 health or
        // changing its color otherwise.
        public void Hit()
        {
            //// set the particle system to interpolate between two colors; in this case, we give
            //// it our self.color but with varying alpha; brighter for higher tiers, fading to 0
            //// over the particle's lifetime (the second color)
            //particleSystem.SetColors(
            //    paletteColors[color].R,
            //    paletteColors[color].G,
            //    paletteColors[color].B,
            //    55 * (tier + 1),
            //    paletteColors[color].R,
            //    paletteColors[color].G,
            //    paletteColors[color].B,
            //    0
            //);

            //particleSystem.Emit(64);

            // sound on hit
            Breakout.Instance.Sounds["brick-hit-2"].Stop();
            Breakout.Instance.Sounds["brick-hit-2"].Play();

            // if we're at a higher tier than the base, we need to go down a tier
            // if we're already at the lowest color, else just go down a color
            if (Tier > 0)
            {
                if (Color == 0)
                {
                    Tier--;
                    Color = 4;
                }
                else
                {
                    Color--;
                }
            }
            else
            {
                // if we're in the first tier and the base color, remove brick from play
                if (Color == 0)
                {
                    InPlay = false;
                }
                else
                {
                    Color--;
                }
            }

            // play a second layer sound if the brick is destroyed
            if (!InPlay)
            {
                Breakout.Instance.Sounds["brick-hit-1"].Stop();
                Breakout.Instance.Sounds["brick-hit-1"].Play();
            }
        }

        public void Update(TimeSpan dt)
        {
            //particleSystem.Update(dt);
        }

        public void Render()
        {
            if (InPlay)
            {
                Breakout.Instance.Graphics.Draw(Breakout.Instance.Textures["main"],
                    // multiply color by 4(-1) to get our color offset, then add tier to that
                    // to draw the correct tier and color brick onto the screen)
                    Breakout.Instance.Frames["bricks"][Color * 4 + Tier],
                    X, Y);
            }
        }

        //// Need a separate render function for our particles so it can be called after all bricks are drawn;
        //// otherwise, some bricks would render over other bricks' particle systems.
        //public void RenderParticles()
        //{
        //    Breakout.Instance.Draw(particleSystem, X + 16, Y + 8);
        //}
    }
}

using GameEngine.States;
using LegendOf50;
using LegendOf50Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegendOf50Game
{
    // Entity Class
    public class Entity
    {
        public Direction Direction { get; set; }

        public Dictionary<string, Animation> Animations { get; }

        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; }
        public int Height { get; }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public int WalkSpeed { get; }

        public int Health { get; private set; }

        public bool Invulnerable { get; private set; }
        public TimeSpan InvulnerableDuration { get; private set; }
        public TimeSpan InvulnerableTimer { get; private set; }
        public TimeSpan FlashTimer { get; private set; }

        public bool Dead { get; set; }

        public StateMachine StateMachine { get; set; }

        public Animation CurrentAnimation { get; private set; }

        public Entity(Dictionary<string, Animation> animations, int walkSpeed, double x, double y, int width, int height, int health, int offsetX = 0, int offsetY = 0)
        {
            Direction = Direction.Down;

            Animations = CreateAnimations(animations);

            // dimensions
            X = x;
            Y = y;
            Width = width;
            Height = height;

            // drawing offsets for padded sprites
            OffsetX = offsetX;
            OffsetY = offsetY;

            WalkSpeed = walkSpeed;

            Health = health;

            // flags for flashing the entity when hit
            Invulnerable = false;
            InvulnerableDuration = TimeSpan.Zero;
            InvulnerableTimer = TimeSpan.Zero;
            FlashTimer = TimeSpan.Zero;

            Dead = false;
        }

        public Dictionary<string, Animation> CreateAnimations(Dictionary<string, Animation> animations)
        {
            return animations.ToDictionary(animation => animation.Key, animation => new Animation(animation.Value.Frames, animation.Value.Interval, animation.Value.Texture ?? "entities"));
        }

        // AABB with some slight shrinkage of the box on the top side for perspective.
        public virtual bool Collides(Entity target)
        {
            return !(X + Width < target.X || X > target.X + target.Width ||
                     Y + Height < target.Y || Y > target.Y + target.Height);
        }

        // AABB with some slight shrinkage of the box on the top side for perspective.
        public virtual bool Collides(GameObject @object)
        {
            return !(X + Width < @object.X || X > @object.X + @object.Width ||
                     Y + Height < @object.Y || Y > @object.Y + @object.Height);
        }

        // AABB with some slight shrinkage of the box on the top side for perspective.
        public virtual bool Collides(Doorway doorway)
        {
            return !(X + Width < doorway.X || X > doorway.X + doorway.Width ||
                     Y + Height < doorway.Y || Y > doorway.Y + doorway.Height);
        }

        // AABB with some slight shrinkage of the box on the top side for perspective.
        public virtual bool Collides(Hitbox hitbox)
        {
            return !(X + Width < hitbox.X || X > hitbox.X + hitbox.Width ||
                     Y + Height < hitbox.Y || Y > hitbox.Y + hitbox.Height);
        }

        public void Damage(int damage)
        {
            Health -= damage;
        }

        public void GoInvulnerable(TimeSpan duration)
        {
            Invulnerable = true;
            InvulnerableDuration = duration;
        }

        public async Task ChangeState(string name)
        {
            await StateMachine.Change(name);
        }

        public void ChangeAnimation(string name)
        {
            CurrentAnimation = Animations[name];
        }

        public async Task Update(TimeSpan dt)
        {
            if (Invulnerable)
            {
                FlashTimer += dt;
                InvulnerableTimer += dt;

                if (InvulnerableTimer > InvulnerableDuration)
                {
                    Invulnerable = false;
                    InvulnerableTimer = TimeSpan.Zero;
                    InvulnerableDuration = TimeSpan.Zero;
                    FlashTimer = TimeSpan.Zero;
                }
            }

            await StateMachine.Update(dt);

            CurrentAnimation?.Update(dt);
        }

        public async Task ProcessAI(Dictionary<string, object> parameters, TimeSpan dt)
        {
            await StateMachine.ProcessAI(parameters, dt);
        }

        public async Task Render(int adjacentOffsetX = 0, int adjacentOffsetY = 0)
        {
            // draw sprite slightly transparent if invulnerable every 0.04 seconds
            if (Invulnerable && FlashTimer > TimeSpan.FromSeconds(0.06))
            {
                FlashTimer = TimeSpan.Zero;
                LegendOf50.Instance.Graphics.SetColor(255, 255, 255, 64);
            }

            (X, Y) = (X + adjacentOffsetX, Y + adjacentOffsetY);
            await StateMachine.Render();
            LegendOf50.Instance.Graphics.SetColor(255, 255, 255, 255);
            (X, Y) = (X - adjacentOffsetX, Y - adjacentOffsetY);
        }
    }
}

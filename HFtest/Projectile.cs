using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public class Projectile : Sprite
    {
        public float linearVelocity { get; private set; }
        public Vector2 Direction { get; set; }
        public Vector2 Origin { get; set; }
        public Agent Parent { get; set; }
        public Weapon weaponFrom { get; private set; }
        public float Lifespan { get; private set; }
        public float Timer { get; set; }

        public Projectile(Vector2 Position, Vector2 Direction, string textureName, float speed, Weapon weapon, float lifespan) : base(Position, textureName)
        {
            linearVelocity = speed;
            this.Direction = Direction;
            weaponFrom = weapon;
            Lifespan = lifespan;

        }

        public override object Clone()
        {
            //returns a projectile with the same characteristics as this one
            //used so all projectiles from one weapon are the same
            return new Projectile(
                new Vector2(0, 0),
                new Vector2(0, 0),
                this.textureName,
                this.linearVelocity,
                this.weaponFrom,
                this.Lifespan
                );
        }

        public void SetVelocity()
        {
            //normalise the direction to have a magnitude of 1
            float scalefactor = 1f / MathF.Sqrt(Direction.X * Direction.X + Direction.Y * Direction.Y);
            Direction = new Vector2(Direction.X * scalefactor, Direction.Y * scalefactor);
            //make projectile travel at its defined speed
            Velocity = Direction * linearVelocity;
        }

        public override void Update(GameTime gameTime)
        {
            Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (Timer > Lifespan)
            {
                //if projectile has been around longer than its designated lifespan then it should be deleted
                isRemoved = true;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rectangle, Color.White);
        }
    }
}

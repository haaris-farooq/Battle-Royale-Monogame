using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public class Weapon : Sprite
    {
        public WeaponType weaponType { get; private set; }
        public float Weight { get; private set; }
        public float firingCooldown { get; private set; }
        public float Power { get; private set; }        
        public Agent Holder { get; private set; }

        private float timeSinceLastShot;
        private Projectile baseProjectile;

        //Constructor used for loading a new weapon
        public Weapon(Vector2 Position, WeaponType weaponType, string textureName, float power, float cooldown, float weight, string projTexture, float projLifespan, float projSpeed, Agent holder) : base(Position, textureName)
        {
            this.weaponType = weaponType;
            this.Power = power;
            this.firingCooldown = cooldown;
            this.Weight = weight;
            this.Holder = holder;
            CreateBaseProjectile(projTexture, projSpeed, projLifespan);
        }

        //Constructor used for cloning weapon
        public Weapon(Vector2 Position, WeaponType weaponType, string textureName, float power, float cooldown, float weight, Projectile baseProjectile) : base(Position, textureName)
        {
            this.weaponType = weaponType;
            this.Power = power;
            this.firingCooldown = cooldown;
            this.Weight = weight;
            this.Holder = Holder;
            this.baseProjectile = baseProjectile;
            //so weapon can be shot immediately upon picking up
            timeSinceLastShot = firingCooldown;
        }

        private void CreateBaseProjectile(string texturename, float speed, float lifespan)
        {
            baseProjectile = new Projectile(
                new Vector2(0, 0),
                new Vector2(0, 0),
                texturename,
                speed,
                this,
                lifespan
                );
        }

        public override object Clone()
        {
            return new Weapon(
                Vector2.Zero,
                this.weaponType,
                this.textureName,
                this.Power,
                this.firingCooldown,
                this.Weight,
                this.baseProjectile
                );
        }

        public Projectile GetCloneOfBaseProjectile()
        {
            return (Projectile)this.baseProjectile.Clone();
        }

        public float CalculateWeaponRange()
        {
            //calculates range based on speed and lifespan of each projectile
            return baseProjectile.Lifespan * baseProjectile.linearVelocity;
        }

        public void ResetTime()
        {
            timeSinceLastShot = 0;
        }

        public bool ReadyToShoot()
        {
            return timeSinceLastShot >= firingCooldown;
        }

        public int TimeLeftTillShoot()
        {
            //returns the time left till this weapon can shoot
            return Math.Clamp((int)(firingCooldown - timeSinceLastShot), 0, (int)firingCooldown);
        }

        public void UpdateHolder(Agent newHolder)
        {
            Holder = newHolder;
        }

        public override void Update(GameTime gameTime)
        {
            if (Holder != null)
            {
                Position = Holder.Position + Holder.weaponOffset;
                timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {  
            if (Holder != null)
            {
                Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
                spriteBatch.Draw(Texture, Rectangle, null, Color.White, Holder.Rotation + (float)Math.PI / 2, origin, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(Texture, Rectangle, Color.White);
            }           
        }
    }
}

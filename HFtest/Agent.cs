using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public abstract class Agent : Sprite
    {
        private const float sixth = 0.17f;
        public float Speed { get; protected set; }
        public float Health { get; protected set; }
        public float maxHealth { get; protected set; }
        public int Kills { get; set; }
        public Weapon equippedWeapon { get; protected set; }
        public bool IsShooting { get; protected set; }
        public bool IsPickingUp { get; protected set; }
        public Vector2 weaponOffset { get => new Vector2(Texture.Width * sixth, Texture.Height * sixth); }
        public Vector2 projectileDirection { get; protected set; }

        protected float pickingCooldown;
        protected float timeSinceLastPicked;

        public float Rotation { get; protected set; }    

        public Agent(Vector2 position, string textureName, float speed, float health) : base(position, textureName)
        {
            Speed = speed;
            maxHealth = health;
            Health = maxHealth;            
            IsShooting = false;
            IsPickingUp = false;
            pickingCooldown = 500;
            timeSinceLastPicked = 0;
        }
        protected void MoveLeft()
        {
            Velocity = new Vector2(-Speed, Velocity.Y);
        }
        protected void MoveRight()
        {
            Velocity = new Vector2(Speed, Velocity.Y);
        }
        protected void MoveUp()
        {
            Velocity = new Vector2(Velocity.X, -Speed);
        }
        protected void MoveDown()
        {
            Velocity = new Vector2(Velocity.X, Speed);
        }
        protected virtual void FireProjectile()
        {
            IsShooting = true;
        }

        public virtual void TakeDamage(float dmg)
        {
            //deal damage to the agent
            Health -= dmg;
            //ensure health does not go below 0
            Health = Health = Math.Clamp(Health, 0, maxHealth);
            if(Health == 0)
            {
                //if agent is dead then remove it from the game
                isRemoved = true;
            }
        }

        public void UpdateWeapon(Weapon weapon)
        {
            equippedWeapon = weapon;
        }
        public void CollectHealth(Collectible health)
        {
            //if agent is picking up a collectible then add its value to the agents health
            Health += health.Value;
            //ensure agents health does not go above its max health
            Health = Math.Clamp(Health, 0, maxHealth);
        }

        protected void ApplyWeightToSpeed()
        {
            //adjust speed depending on weight of the current weapon
            //speed is not changed if no weapon is held
            if (equippedWeapon != null)
            {
                Velocity *= equippedWeapon.Weight;
            }
        }

        public bool ReadyToPick()
        {
            //returns true if the time since the agent last picked up or dropped a weapon is longer than the cooldown
            return timeSinceLastPicked >= pickingCooldown;
        }


        public void ResetTimeSinceLastPicked()
        {
            timeSinceLastPicked = 0;
        }

        public override void Update(GameTime gameTime)
        {
            ApplyWeightToSpeed();
            timeSinceLastPicked += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //draws agent using its position and rotation
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            spriteBatch.Draw(Texture, Rectangle, null, Color.White, Rotation, origin, SpriteEffects.None, 0);
        }

        


    }
}

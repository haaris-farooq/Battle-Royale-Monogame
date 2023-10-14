using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public class Player : Agent
    {
        private Input Input;

        public float damageTaken { get; set; }
        

        public Player(Vector2 position, string textureName, float speed, float health) : base(position, textureName, speed, health)
        {
            Input = new Input()
            {
                Up = Keys.W,
                Down = Keys.S,
                Left = Keys.A,
                Right = Keys.D,
            };
            
        }
        public void GetActionFromInput()
        {            
            if (Keyboard.GetState().IsKeyDown(Input.Left))
                MoveLeft();
            else if (Keyboard.GetState().IsKeyDown(Input.Right))
                MoveRight();

            if (Keyboard.GetState().IsKeyDown(Input.Up))
                MoveUp();
            else if (Keyboard.GetState().IsKeyDown(Input.Down))
                MoveDown();

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                IsPickingUp = true;
            else
                IsPickingUp = false;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                FireProjectile();
            else
                IsShooting = false;

        }

        public override void TakeDamage(float dmg)
        {
            base.TakeDamage(dmg);
            damageTaken += dmg;
        }

        private void GetRotationFromMouse()
        {
            MouseState ms = Mouse.GetState();
            //Vector between mouse position and the current player position
            Vector2 rotationDirection = new Vector2(ms.X, ms.Y) - Vector2.Transform(Position, World.Camera.Transform);
            //calculate rotation used Atan function
            Rotation = (float)Math.Atan2(rotationDirection.Y, rotationDirection.X);
            //adjust rotation to make player face mouse
            Rotation -= (float)(Math.PI) / 2;
        }
        protected override void FireProjectile()
        {
            MouseState ms = Mouse.GetState();
            //make projectile fire in the direction of mouse click
            projectileDirection = new Vector2(ms.X, ms.Y) - Vector2.Transform(Position, World.Camera.Transform);
            base.FireProjectile();
        }
        public override void Update(GameTime gameTime)
        {
            GetActionFromInput();
            GetRotationFromMouse();
            base.Update(gameTime);
        }


    }
}

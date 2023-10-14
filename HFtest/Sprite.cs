using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public abstract class Sprite
    {
        public Vector2 Position { get; set; }
        public Point worldPosition
        {
            get => new Point((int)(Position.X / World.tileSize), (int)(Position.Y / World.tileSize));
        }
        public Texture2D Texture { get; set; }
        public string textureName { get; set; }
        public bool isRemoved { get; set; }
        public Vector2 Velocity { get; set; }

        public Sprite(Vector2 Position, string textureName)
        {
            this.Position = Position;
            this.textureName = textureName;
            isRemoved = false;
        }
        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);


        public virtual object Clone()
        {
            //overridable sub allowing sprites to be cloned
            //used for dynamically creating objects that need to have similar characteristics as an existing sprite
            return null;
        }

        public void LoadTexture(ContentManager contentManager)
        {
            //gives the sprite a texture by loading it from its file name
            try
            {
                Texture = contentManager.Load<Texture2D>(textureName);
            }
            catch (Exception Ex)
            {
                Game1.gameState = GameState.Error;
                Game1.errorText = Ex.Message;
            }


        }
        public Rectangle Rectangle
        {
            //rectangle of sprite based on position and size of texture
            //used for collisions
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }

    }
}

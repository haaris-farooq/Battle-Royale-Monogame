using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public class Collectible : Sprite
    {
        public int Value { get; set; }
        public Collectible(Vector2 Position, string textureName, int value) : base(Position, textureName)
        {
            //how much health is given when this collectible is collected
            Value = value;
        }

        public override object Clone()
        {
            return new Collectible(Vector2.Zero, this.textureName, this.Value);
        }
        public override void Update(GameTime gameTime)
        {}

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rectangle, Color.White);
        }      
    }
}

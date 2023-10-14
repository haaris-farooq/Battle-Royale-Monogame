using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public class GameScreen
    {
        protected Texture2D backgroundImage;

        protected List<string> Items;

        protected Vector2 textPosition;

        protected float itemSpacing;

        protected SpriteFont font;

        public GameScreen(Texture2D backgroundImage, List<string> items, Vector2 textPosition, float itemSpacing, SpriteFont font)
        {
            this.backgroundImage = backgroundImage;
            Items = items;
            this.textPosition = textPosition;
            this.itemSpacing = itemSpacing;
            this.font = font;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //draw background texture
            spriteBatch.Draw(backgroundImage, Vector2.Zero, Color.White);
            //set start position of text
            Vector2 currentTextPosition = textPosition;

            for (int i = 0; i < Items.Count; i++)
            {
                //draw current item
                spriteBatch.DrawString(font, Items[i], currentTextPosition, Color.White);
                //increment position to draw next item
                currentTextPosition.Y += font.LineSpacing + itemSpacing;
            }
        }
    }
}

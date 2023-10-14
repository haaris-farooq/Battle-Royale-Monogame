using HFtest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public class MenuScreen : GameScreen
    {
        // The index of the currently selected menu item.
        private int selectedIndex;

        // The color of the selected menu item.
        private Color selectedColor;

        // The color of the unselected menu items.
        private Color unselectedColor;


        public MenuScreen(Texture2D backgroundImage, List<string> items, Vector2 textPosition, float itemSpacing, SpriteFont font) : base(backgroundImage, items, textPosition, itemSpacing, font)
        {
            selectedColor = Color.Yellow;
            unselectedColor = Color.White;
        }

        public int GetSelectedItem()
        {
            return selectedIndex;
        }

        public void Update(GameTime gameTime)
        {
            // Handle input from the player to navigate the menu.
            if (KeyboardManager.IsKeyPressedOnce(Keys.Up))
            {
                selectedIndex--;

                if (selectedIndex < 0)
                {
                    selectedIndex = Items.Count - 1;
                }
            }
            else if (KeyboardManager.IsKeyPressedOnce(Keys.Down))
            {
                selectedIndex++;

                if (selectedIndex >= Items.Count)
                {
                    selectedIndex = 0;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw background texture
            spriteBatch.Draw(backgroundImage, Vector2.Zero, Color.White);

            // Calculate the position of the first menu item.
            Vector2 currentPosition = textPosition;
            currentPosition.X -= 10;

            // Iterate over the menu items and draw them to the screen.
            for (int i = 0; i < Items.Count; i++)
            {
                // Use the selected color for the selected menu item, and the unselected
                // color for the other menu items.
                Color color = (i == selectedIndex) ? selectedColor : unselectedColor;

                spriteBatch.DrawString(font, Items[i], currentPosition, color);

                // Increment the position of the next menu item.
                currentPosition.Y += font.LineSpacing + itemSpacing;
            }
        }
    }

}

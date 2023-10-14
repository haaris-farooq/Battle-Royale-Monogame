using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFtest
{
    public static class KeyboardManager
    {
        //class for checking if a key has been pressed once
        //can be expanded for checking if a key has been held down but not necessary for this project
       
        private static KeyboardState currentKeyState;
        private static KeyboardState previousKeyState;

        public static void Update()
        {
            //sets the current key state to what the keyboard is right now
            currentKeyState = Keyboard.GetState();
        }
        public static void UpdatePrevious()
        {
            //sets the previous key state so we can check against it
            previousKeyState = currentKeyState;
        }
        public static bool IsKeyPressedOnce(Keys key)
        {
            //checks if a key has only been pressed this frame and not the previous one
            return currentKeyState.IsKeyDown(key) && previousKeyState.IsKeyUp(key);
        }
    }
}

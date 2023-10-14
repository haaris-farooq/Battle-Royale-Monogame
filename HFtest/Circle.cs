using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ComputingProjectHF
{
    class Circle
    {
        public Vector2 Centre { get; private set; }
        public float Radius { get; private set; }

        public Circle(Sprite sprite)
        {
            //calculate centre by offsetting top left position of sprite
            Centre = new Vector2(sprite.Position.X + sprite.Texture.Width / 2, sprite.Position.Y + sprite.Texture.Height / 2);
            //calculate radius by taking an average of the sprites width and height
            Radius = (sprite.Texture.Width + sprite.Texture.Height) / 2;
        }

        public bool Intersects(Circle circle)
        {
            //get the vector between the centres of the two circles
            Vector2 vectorDistance = circle.Centre - this.Centre;

            //calculate the straight line distance between the two centres
            float distance = (float)Math.Sqrt(Math.Pow(vectorDistance.X, 2) + Math.Pow(vectorDistance.Y, 2));

            //return true if the distance is smaller than the sum of the circles radius'
            return distance <= (this.Radius + circle.Radius);
        }
    }
}

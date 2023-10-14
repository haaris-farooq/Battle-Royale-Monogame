using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public class Camera
    {
        public Vector2 Position { get; private set; }
        public float Zoom { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Camera(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            Zoom = 1;
        }

        public Vector2 Centre
        {
            get { return new Vector2(Width * 0.5f, Height * 0.5f); }
        }

        public Matrix Transform
        {
            //creates a matrix based on the position of the camera used to transform the spritebatch
            //also used to turn current screen positions to wider screen positions (e.g. transforming mouse clicks to their actual position)
            get 
            {
                //create an offset of where the camera is 
                //use minus of the position as offset to simulate a camera moving
                //adjust scale using zoom if needed
                //everything is drawn using the position of the camera as its centre
                return Matrix.CreateTranslation(-(int)Position.X, -(int)Position.Y, 0)
                  * Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))
                  * Matrix.CreateTranslation(new Vector3(Centre, 0));
            }
        }

        public void ClampCamera(Map Map)
        {
            //camera should stop moving when player has reached the end of the map
            var CameraMax = new Vector2((float)Map.mapWidth * Map.tileSize - (Width / 2), (float)Map.mapHeight * Map.tileSize - (Height/2));
            Position = Vector2.Clamp(Position, new Vector2((float)(Game1.ScreenWidth / 2), (float)(Game1.ScreenHeight / 2)), CameraMax);
        }

        public void Update(Player player, Map Map)
        {
            //centres camera on the player
            Position = player.Position;
            //stops camera moving so the outside of the map is not visible
            ClampCamera(Map);
        }
    }
}

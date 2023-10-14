using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public class Tile
    {
        public string tileID { get; set; }
        public Point Position { get; set; }
        public Texture2D Texture { get; private set; }
        public string textureName { get; private set; }
        public bool isSolid { get; private set; }

        //Constructor used for loading Tile templates
        public Tile(string TileID, string TextureName, bool IsSolid)
        {
            tileID = TileID;
            textureName = TextureName;
            isSolid = IsSolid;
        }


        //Constructor used for dynamically creating Tiles
        public Tile(string TileID, Point Position, string TextureName, bool IsSolid)
        {
            tileID = TileID;
            this.Position = Position;
            textureName = TextureName;
            isSolid = IsSolid;
        }

        public void UpdateTexture(Texture2D texture)
        {
            Texture = texture;
        }

        public void LoadTexture(ContentManager content)
        {
            Texture2D tex = content.Load<Texture2D>(textureName);
            Texture = tex;
        }

        public void MakeSolid()
        {
            isSolid = true;
        }
    }
}

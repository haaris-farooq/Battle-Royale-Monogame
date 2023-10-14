using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputingProjectHF
{
    public class Map
    {
        public Tile[,] tileMap { get; set; }
        public int mapHeight { get; set; }
        public int mapWidth { get; set; }
        public int tileSize { get; set; }

        private Texture2D enclosedTexture;

        private int timesEnclosed;       

        public void CreateEnclosedTexture()
        {
            //create texture used for displaying tiles that have been marked out of bounds of the map
            enclosedTexture = Game1.GenerateTexture(tileSize, tileSize, Color.Red);
            timesEnclosed = 0;
        }
        public bool IsTileBlocked(int x, int y)
        {
            //function for checking if a tile can be moved through or not
            return tileMap[x,y].isSolid;
        }

        public bool IsTileInBounds(int x, int y)
        {
            //function for checking if a tile is within the bounds of the map
            return x < tileMap.GetUpperBound(0) && x >= 0 && y < tileMap.GetUpperBound(1) && y >= 0;
        }

        public Point GetRandomValidSquare()
        {
            Random r = new Random();
            Point chosenPoint = new Point(-1, -1);
            bool valid = false;
            while (valid == false)
            {
                chosenPoint = new Point(r.Next(1, mapWidth), r.Next(1, mapHeight));
                if (IsTileInBounds(chosenPoint.X, chosenPoint.Y))
                {
                    if (IsTileBlocked(chosenPoint.X, chosenPoint.Y) == false)
                    {
                        valid = true;
                    }
                }
            }
            return chosenPoint;
        }

        public List<Tile> GetWalkableTiles(Point currentTile)
        {        
            //returns a list of neighbouring tiles to a tile
            //only tiles that are not solid and can be moved through are returned
            //used for enemy pathfinding
            Point upperTile = Point.Plus(currentTile, new Point(1, 1));
            Point lowerTile = Point.Plus(currentTile, new Point(-1, -1));
            List<Tile> maplist = ConvertMapToList();

            return maplist.Where(s =>
            s.isSolid == false
            && s.Position.X >= lowerTile.X
            && s.Position.X <= upperTile.X
            && s.Position.Y >= lowerTile.Y
            && s.Position.Y <= upperTile.Y
            && s.Position.Equals(currentTile) == false).ToList();
        }       

        public Point ScreenToWorld(Vector2 ScreenPosition)
        {
            //converts a Vector screen coordinate to a Map position
            return new Point((int)ScreenPosition.X / tileSize, (int)ScreenPosition.Y / tileSize);
        }

        public Vector2 WorldToScreen(Point WorldPosition)
        {
            //converts a Map position to a Vector screen coordinate
            return new Vector2(WorldPosition.X * tileSize, WorldPosition.Y * tileSize);
        }

        public void EncloseMap()
        {
            if (timesEnclosed >= tileMap.GetUpperBound(0) - 1 || timesEnclosed >= tileMap.GetUpperBound(1) - 1)
            {
                //only close the map up to a certain point
                //dont want map to be fully enclosed so there is nothing remaining
                return;
            }
            for (int i = 0 + timesEnclosed; i <= tileMap.GetUpperBound(0) - timesEnclosed; i++)
            {
                //Sets tiles to enclosed depending on how many times we have done this process already
                //Goes through and sets the outermost horizontal rows to enclosed
                SetTileToUnwalkable(tileMap[i, 0 + timesEnclosed]);
                SetTileToUnwalkable(tileMap[i, tileMap.GetUpperBound(0) - timesEnclosed]);
            }
            for (int j = 0 + timesEnclosed; j <= tileMap.GetUpperBound(1) - timesEnclosed; j++)
            {
                //goes through and sets the outermost vertical rows to enclosed
                SetTileToUnwalkable(tileMap[0 + timesEnclosed, j]);
                SetTileToUnwalkable(tileMap[tileMap.GetUpperBound(1) - timesEnclosed, j]);
            }
            //increments the number of times we have done this process so we can enclose the next set of tiles next time
            timesEnclosed++;
        }

        private void SetTileToUnwalkable(Tile tile)
        {
            //makes a tile out of bounds of the map by displaying it as cordoned off
            tile.UpdateTexture(enclosedTexture);
            //means the tile can no longer be moved through
            tile.MakeSolid();
        }

        public bool IsTileEnclosed(Point checkPoint)
        {
            return tileMap[checkPoint.X, checkPoint.Y].Texture == enclosedTexture;
        }

        public void LoadTileTextures(ContentManager content)
        {
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    tileMap[i, j].LoadTexture(content);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //draws the entire map
            for (int x = 0; x < mapWidth;x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if(tileMap[x,y].Texture != null)
                    {                       
                        spriteBatch.Draw(tileMap[x,y].Texture, new Vector2(x * tileSize, y   * tileSize), Color.White);                                         
                    }
                }
            }
        }

        private List<Tile> ConvertMapToList()
        {
            List<Tile> tiles = new List<Tile>();
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    tiles.Add(tileMap[x, y]);
                }
            }
            return tiles;
        }

    }
}

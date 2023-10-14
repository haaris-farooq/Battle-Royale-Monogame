using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputingProjectHF
{
    public class CollisionManager
    {
        public void Update(GameTime gameTime, Map Map, List<Sprite> Sprites)
        {
            for (int i = 0; i < Sprites.Count; i++)
            {
                //move sprite horizontally based on their velocity
                Sprites[i].Position = new Vector2(Sprites[i].Position.X + Sprites[i].Velocity.X, Sprites[i].Position.Y);
                if (IsMoveAllowed(Sprites[i], Map) == false)
                {
                    //if x movement results in colliding with a tile or going out of bounds then disallow it
                    OnCollisionWithTileX(Sprites[i]);
                }
                //move sprite vertically based on their velocity
                Sprites[i].Position = new Vector2(Sprites[i].Position.X, Sprites[i].Position.Y + Sprites[i].Velocity.Y);
                if (IsMoveAllowed(Sprites[i], Map) == false)
                {
                    //if y movement results in colliding with a tile or going out of bounds then disallow it
                    OnCollisionWithTileY(Sprites[i]);
                }

                if (Sprites[i] is Projectile)
                {
                    //check if projectile has collided with anything
                    CheckForProjectileCollisions(Map, (Projectile)Sprites[i], Sprites);
                }
                if (Sprites[i] is Agent)
                {
                    //reset velocity back to zero so agent will stop moving
                    //agent should only move for as long as they want to
                    Sprites[i].Velocity = Vector2.Zero;
                }
            }

            foreach (var sprite in Sprites)
            {
                //if a sprite is in the part of the map that has now been cordoned off
                if (Map.IsTileEnclosed(sprite.worldPosition))
                {
                    
                    if (sprite is Weapon == false)
                    {
                        //remove them if they are not a weapon
                        sprite.isRemoved = true;
                    }
                    else if (((Weapon)sprite).Holder == null)
                    {
                        //remove them if they are a weapon and they are not equipped
                        sprite.isRemoved = true;
                    }

                }
            }
            //get rid of all sprites that have been removed from the game so they are no longer updated or drawn
            Sprites.RemoveAll(s => s.isRemoved == true);
        }


        private bool IsMoveAllowed(Sprite sprite, Map Map)
        {
            //checks if the sprite is in the bounds of the map and is on a non-solid tile
            //if they are then this function will return true and if not then false will be returned
            if (Map.IsTileInBounds(sprite.worldPosition.X, sprite.worldPosition.Y) && sprite.Position.X > 0 && sprite.Position.Y > 0)
            {
                if (Map.IsTileBlocked(sprite.worldPosition.X, sprite.worldPosition.Y) == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
                
        }
        private void OnCollisionWithTileY(Sprite sprite)
        {
            if (sprite is Projectile)
            {
                sprite.isRemoved = true;
            }
            //undo y movement of sprite
            sprite.Position = new Vector2(sprite.Position.X, sprite.Position.Y - sprite.Velocity.Y);
            //set velocity in y direction to 0
            sprite.Velocity = new Vector2(sprite.Velocity.X, 0);
        }

        private void OnCollisionWithTileX(Sprite sprite)
        {
            if (sprite is Projectile)
            {
                //if a projectile has collided with a solid tile they are removed from the game
                sprite.isRemoved = true;
            }
            //undo x movement of sprite
            sprite.Position = new Vector2(sprite.Position.X - sprite.Velocity.X, sprite.Position.Y);
            //set velocity in x direction to 0
            sprite.Velocity = new Vector2(0, sprite.Velocity.Y);
        }

        private void CheckForProjectileCollisions(Map Map, Projectile projectile, List<Sprite> Sprites)
        {

            if (projectile.isRemoved == true)
            {
                return;
            }
            //split the world into a smaller chunk based on the projectile position
            //only check for collisions between this projectile and agents in this chunk
            Point radius = Map.ScreenToWorld(new Vector2(Game1.ScreenWidth / 4, Game1.ScreenHeight / 4));
            List<Sprite> collisionable = World.GetSpritesInRange(projectile.worldPosition, radius.X, radius.Y, Sprites, true);

            foreach (Sprite s in collisionable)
            {
                //check if agent and projectile have collided
                if (s.Rectangle.Intersects(projectile.Rectangle))
                {
                    //ensure agents do not take damage from their own projectiles
                    if (projectile.Parent != s)
                    {
                        ((Agent)s).TakeDamage(projectile.weaponFrom.Power);
                        projectile.isRemoved = true;
                        if (s.isRemoved == true)
                        {
                            //increment the number of kills of the agent who shot the bullet if projectile results in a death
                            projectile.Parent.Kills += 1;
                        }
                    }
                }
            }
        }
    }
}

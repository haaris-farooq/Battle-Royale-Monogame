using HFtest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputingProjectHF
{
    public class Enemy : Agent
    {
        public int tileVision { get; set; }

        private const int fifteenth = 15;

        private List<PathfindingTile> activeTiles;
        private List<PathfindingTile> visitedTiles;
        private List<PathfindingTile> path;
        private Point currentTarget;
        private Weapon collectingWeapon;
        private Vector2 Direction;
        private Vector2 previousPosition;
        private float stillTime;
        private bool randomMovement;
        private float currentDirectionTimer;
        private float maxDirectionTimer;
        private float timeTillPathfind;
        private WeaponBias Bias;
        private bool followingPath;       

        public Enemy(Vector2 Position, string textureName, float speed, float health, WeaponBias bias, int tileVision, float directionTimer, float stillTimer) : base(Position, textureName, speed, health)
        {
            Bias = bias;
            maxDirectionTimer = directionTimer;
            currentDirectionTimer = maxDirectionTimer;
            timeTillPathfind = stillTimer;
            randomMovement = true;
            this.tileVision = tileVision;
            stillTime = 0;
            activeTiles = new List<PathfindingTile>();
            visitedTiles = new List<PathfindingTile>();
            path = new List<PathfindingTile>();
            followingPath = false;
        }

        public override object Clone()
        {
            return new Enemy(
                Vector2.Zero,
                this.textureName,
                this.Speed,
                this.Health,
                this.Bias,
                this.tileVision,
                this.maxDirectionTimer,
                this.timeTillPathfind);
        }

        private void Pathfind(Map Map)
        {            
            activeTiles.Clear();
            visitedTiles.Clear();
            path.Clear();
            //Create a new PathfindingTile object at the enemy's current position,
            PathfindingTile start = new PathfindingTile { Position = new Point((int)this.worldPosition.X, (int)this.worldPosition.Y) };
            start.CalculateDistance(currentTarget);
            activeTiles.Add(start);

            //Keep looping until there are no more active tiles to process or the visited tiles list has become too big
            //When visited tiles list reaches a defined limit then abandon pathfinding
            int tileLimit = (Map.tileMap.GetUpperBound(0) * Map.tileMap.GetUpperBound(1)) / fifteenth;
            while (activeTiles.Any() && visitedTiles.Count < tileLimit)
            {
                // Get the active tile with the lowest CostDistance value
                var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

                // Add the current tile to the visited tiles list, and remove it from the
                // active tiles list
                visitedTiles.Add(checkTile);
                activeTiles.Remove(checkTile);

                // If this tile is the target position, we are done
                if (Point.Equals(checkTile.Position, currentTarget))
                {
                    break;
                }

                // Get the walkable tiles for the current tile
                var walkableTiles = Map.GetWalkableTiles(checkTile.Position);

                // Create a list of PathfindingTile objects based on the walkable tiles
                var PathWalkableTiles = GetWalkableTilesForPathfinding(walkableTiles, checkTile);

                // Iterate over each walkable tile
                foreach (var walkableTile in PathWalkableTiles)
                {
                    //We have already visited this tile
                    if (visitedTiles.Any(x => Point.Equals(x.Position, walkableTile.Position)))
                        continue;

                    //Tile is already in the active list but maybe this tile has a better value than the previous tile
                    if (activeTiles.Any(x => Point.Equals(x.Position, walkableTile.Position)))
                    {
                        var existingTile = activeTiles.First(x => Point.Equals(x.Position, walkableTile.Position));
                        if (existingTile.CostDistance > checkTile.CostDistance)
                        {
                            activeTiles.Remove(existingTile);
                            activeTiles.Add(walkableTile);
                        }
                    }
                    else
                    {
                        //We've never seen this tile before so add it to the list. 
                        activeTiles.Add(walkableTile);
                    }
                }
            }
            
        }
        private List<PathfindingTile> GetWalkableTilesForPathfinding(List<Tile> walkableTiles, PathfindingTile checkTile)
        {
            List<PathfindingTile> PathWalkableTiles = new List<PathfindingTile>();

            foreach (var tile in walkableTiles)
            {
                // Create a new PathfindingTile object based on the walkable tile.
                PathWalkableTiles.Add(new PathfindingTile
                {
                    Position = tile.Position,
                    Parent = checkTile,
                    Cost = checkTile.Cost + 1
                });
            }
            foreach (var tile in PathWalkableTiles)
            {
                //set the distance from this tile to the target tile
                tile.CalculateDistance(currentTarget);
            }

            return PathWalkableTiles;
        }

        private void GetPathToTarget()
        {

            //Get the tile at the target position
            var targetTile = visitedTiles.FirstOrDefault(x => Point.Equals(x.Position, currentTarget));

            //If there is no target tile, return an empty path
            if (targetTile == null)
            {
                MoveRandomly();
                return;
            }

            //Create a reference to the current tile, starting with the target tile
            var currentTile = targetTile;

            //Keep looping until we reach the starting position
            while (Point.Equals(currentTile.Position, this.worldPosition) == false)
            {
                //Add the current tile to the beginning of the path list
                path.Insert(0, currentTile);

                //Set the current tile to its parent
                currentTile = currentTile.Parent;

            }
        }

        private PathfindingTile GetNextTileInPath()
        {            

            //If the path is not null and has at least one tile in it return the first tile
            if (path != null && path.Any())
            {
                return path.First();
            }

            //No next tile was found, so return null
            return null;
        }

        private void GetDirectionFromPath()
        {
            PathfindingTile nextTile = GetNextTileInPath();

            if (nextTile != null)
            {
                if (Point.Equals(worldPosition, currentTarget))
                {
                    //if we have reached the target then no longer need to follow the path
                    followingPath = false;
                }
                else if (Point.Equals(nextTile.Position, worldPosition))
                {
                    //if we have reached this tile then remove it from the path
                    path.RemoveAt(0);
                }
                else
                {
                    //calculate direction by getting the vector from the next tile to the current position
                    Direction = new Vector2(Point.Minus(nextTile.Position, worldPosition).X, Point.Minus(nextTile.Position, worldPosition).Y);
                    //normalise vector so we can ensure enemy moves at its defined speed
                    Direction.Normalize();
                }

            }
            else
            {
                //if no next tile then return to random movement
                MoveRandomly();
            }
        }

        private void MakeDecision(GameTime gameTime, List<Sprite> Sprites, Map Map)
        {
            //method for decision making for enemy every frame
            if (followingPath == true && currentTarget != null)
            {
                //if enemy is currently following a path then simply get the direction using the next tile in the path
                GetDirectionFromPath();
            }
            else if (randomMovement == true)
            {
                //if currently moving in a random direction then increment how long enemy has been in this direction
                currentDirectionTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                //if there is still no sprites in the vicinity of the enemy
                //and the enemy has been travelling in the same direction for a certain period of time
                //change the direction of the enemy in a new random direction
                if (Sprites.Count == 0 && currentDirectionTimer >= maxDirectionTimer)
                {
                    MoveRandomly();
                }
                else if (Sprites.Count > 0)
                {
                    //if there is a sprite in this enemies vicinity then break out of random movement and choose a point to follow
                    currentDirectionTimer = 0;
                    ChoosePointToFollow(Sprites);
                    FollowTarget(Map);
                }
            }
            else if (Sprites.Count > 0)
            {
                //if there is a sprite in this enemies vicinity choose a point to follow
                ChoosePointToFollow(Sprites);
                FollowTarget(Map);
            }
            else if (Sprites.Count == 0)
            {
                //if no sprites then return to random movement
                MoveRandomly();
            }
            //check if enemy has reached target weapon, if so then pick it up
            PickUpWeapon();
            //shoot at an agent if in range
            ShootAtAgent(Sprites.Where(s => s is Agent).ToList());
        }

        private void Move()
        {
            //set velocity so enemy is moving in chosen direction at their defined speed
            Velocity = Speed * Direction;
        }

        private void PickUpWeapon()
        {
            //method for picking up weapon if they collide with it
            if (collectingWeapon != null)
            {
                if (this.Rectangle.Intersects(collectingWeapon.Rectangle))
                {
                    //pick up weapon
                    IsPickingUp = true;
                    //reset back to random movement if enemy has picked up target weapon 
                    MoveRandomly();
                }
                else
                {
                    IsPickingUp = false;
                }
            }
            else
            {
                IsPickingUp = false;
            }
        }


        private void MoveRandomly()
        {
            currentTarget = null;
            collectingWeapon = null;
            followingPath = false;
            randomMovement = true;
            ChooseRandomDirection();
            currentDirectionTimer = 0;
        }

        private void ShootAtAgent(List<Sprite> targets)
        {
            if (equippedWeapon == null || targets.Count == 0)
            {
                //if enemy has no weapon or there is nothing to shoot at then they cannot shoot
                IsShooting = false;
                return;
            }
            else
            {
                if (targets.Count > 0)
                {
                    //choose the agent with the lowest health
                    targets = targets.OrderBy(s => ((Agent)s).Health).ToList();
                    if (equippedWeapon.ReadyToShoot())
                    {
                        //if they can shoot then shoot at the chosen agent
                        //projectile direction calculated using a vector from agent position to this enemy position
                        projectileDirection = targets[0].Position - Position;
                        IsShooting = true;
                    }
                }
            }
        }

        private void ChoosePointToFollow(List<Sprite> targets)
        {
            //method for choosing which sprite to follow
            randomMovement = false;
            List<Sprite> agents = targets.Where(s => s is Agent).ToList();
            List<Sprite> weapons = targets.Where(s => s is Weapon).ToList();
            List<Sprite> collectibles = targets.Where(s => s is Collectible).ToList();
            if (targets.Count > 0)
            {
                //if enemy has no weapon and can see a weapon then prioritise getting a weapon
                if (equippedWeapon == null && weapons.Count > 0)
                {
                    foreach (Weapon w in weapons.Cast<Weapon>())
                    {
                        ChooseWeaponToFollow(w);
                    }
                }
                //if enemy has a weapon then follow the agent with the lowest health
                else if (agents.Count > 0)
                {
                    agents = agents.OrderBy(s => ((Agent)s).Health).ToList();
                    currentTarget = agents[0].worldPosition;
                }
                //if there are no agents but enemy has a weapon then check to see if any weapon in the vicinity
                //is better than their current one
                else if (weapons.Count > 0)
                {
                    foreach (Weapon w in weapons.Cast<Weapon>())
                    {
                        ChooseWeaponToFollow(w);
                    }
                }
                //if no target has been chosen yet then follow the collectible with the highest value in the vicinity
                if (collectibles.Count > 0 && currentTarget == null)
                {
                    collectibles = collectibles.OrderByDescending(s => ((Collectible)s).Value).ToList();
                    currentTarget = collectibles[0].worldPosition;
                }
                //if we get to here then no target has been chosen at all so return to random movement
                else if (currentTarget == null)
                {
                    randomMovement = true;
                }
            }
        }

        private void FollowTarget(Map Map)
        {
            if (currentTarget != null)
            {
                if (stillTime >= timeTillPathfind && followingPath == false)
                {
                    //if enemy has been still for a defined amount of time then start pathfinding to the target
                    followingPath = true;
                    Pathfind(Map);
                    GetPathToTarget();
                    GetDirectionFromPath();
                }
                else
                {
                    if (Point.Equals(currentTarget, worldPosition) == false)
                    {
                        //if we are not at the target point then move in the direction of it
                        Direction = new Vector2(currentTarget.X * World.tileSize, currentTarget.Y * World.tileSize) - Position;
                        Direction.Normalize();
                    }
                    else
                    {
                        //if we have reached the point reset our target to null
                        currentTarget = null;
                        collectingWeapon = null;
                    }
                }
            }
            else
            {
                //if we dont have a target then reset to random movement
                MoveRandomly();
            }
        }
        private void ChooseRandomDirection()
        {
            Random r = new Random();
            switch (r.Next(1, 5))
            {
                case 1:
                    //right
                    Direction = new Vector2(1, 0);
                    break;
                case 2:
                    //down
                    Direction = new Vector2(0, 1);
                    break;
                case 3:
                    //left
                    Direction = new Vector2(-1, 0);
                    break;
                case 4:
                    //up
                    Direction = new Vector2(0, -1);
                    break;
            }

        }

        private void ChooseWeaponToFollow(Weapon weaponToCompare)
        {            
            if (weaponToCompare.Holder != null)
            {
                //if weapon cannot be picked up then cant compare with it
                return;
            }
            if (equippedWeapon == null && collectingWeapon == null)
            {
                //if enemy has no weapon and isnt collecting a weapon then start following this weapon
                collectingWeapon = weaponToCompare;
                currentTarget = collectingWeapon.worldPosition;
            }
            else if (collectingWeapon != null)
            {
                //if enemy is currently collecting a weapon, check to see if this one is better
                //if so then follow this one instead
                collectingWeapon = CompareWeapons(collectingWeapon, weaponToCompare);
                currentTarget = collectingWeapon.worldPosition;
            }
            else if (equippedWeapon != null)
            {
                //if enemy is not collecting a weapon but has a weapon equipped
                //check if this weapon is better then what enemy has equipped
                collectingWeapon = CompareWeapons(equippedWeapon, weaponToCompare);
                if (collectingWeapon == equippedWeapon)
                {
                    //if equipped weapon is better then no need to collect this one
                    collectingWeapon = null;
                    currentTarget = null;
                }
                else
                {
                    //if weapon to compare is better then collect it
                    currentTarget = collectingWeapon.worldPosition;
                }
            }
        }

        private Weapon CompareWeapons(Weapon weapon1, Weapon weapon2)
        {
            //Decision tree for comparing which is better out of two weapons
            int pointsfor1 = 0;
            int pointsfor2 = 0;

            if (weapon1.weaponType == weapon2.weaponType)
            {
                //if they are the same kind of weapon then just return the first weapon
                return weapon1;
            }

            //if weapon has a better range add points
            if (weapon1.CalculateWeaponRange() >= weapon2.CalculateWeaponRange())
            {
                //if the enemy's bias is this characteristic then add 2 points and if not then add 1
                pointsfor1 += (Bias == WeaponBias.Range) ? 2 : 1;
            }
            else
            {
                pointsfor2 += (Bias == WeaponBias.Range) ? 2 : 1;
            }

            //if weapon has a higher fire rate add points
            if (weapon1.firingCooldown <= weapon2.firingCooldown)
            {
                pointsfor1 += (Bias == WeaponBias.Rate) ? 2 : 1;
            }
            else
            {
                pointsfor2 += (Bias == WeaponBias.Rate) ? 2 : 1;
            }

            //if weapon deals more damage add points
            if (weapon1.Power >= weapon2.Power)
            {
                pointsfor1 += (Bias == WeaponBias.Damage) ? 2 : 1;
            }
            else
            {
                pointsfor2 += (Bias == WeaponBias.Damage) ? 2 : 1;
            }

            //if weight is greater then the weapon is lighter (weight is a decimal multiplier applied to speed of the holder)
            if (weapon1.Weight >= weapon2.Weight)
            {
                pointsfor1 += (Bias == WeaponBias.Weight) ? 2 : 1;
            }
            else
            {
                pointsfor2 += (Bias == WeaponBias.Weight) ? 2 : 1;
            }

            //return the weapon with the highest points
            //if both the same, prioritise the first weapon
            return (pointsfor1 >= pointsfor2) ? weapon1 : weapon2;
        }
        public void Update(GameTime gameTime, List<Sprite> spritesInRange, Map Map)
        {
            if (Position == previousPosition)
            {
                //if enemy is still in same position as last frame then increase the amount of time they have been still
                stillTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                //if they have changed position reset the time they have been still
                stillTime = 0;
            }
            //update previous position for checking next frame
            previousPosition = Position;
            //ensure this enemy is not in list when making a decision about movement
            if (spritesInRange.Contains(this))
            {
                spritesInRange.Remove(this);
            }           
            if (equippedWeapon != null)
            {
                if (spritesInRange.Contains(equippedWeapon))
                {
                    //ensure this enemy's current weapon is not in list of sprites when making a decision about movement
                    spritesInRange.Remove(equippedWeapon);
                }
            }
            MakeDecision(gameTime, spritesInRange, Map);
            Move();
            base.Update(gameTime);
        }
    }
}

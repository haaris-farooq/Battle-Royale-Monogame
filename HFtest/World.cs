using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Linq;
using HFtest;

namespace ComputingProjectHF
{
    public class World
    {
        public static int tileSize { get; private set; }
        private readonly string tilePath;
        private readonly string mapPath;
        private readonly string weaponPath;
        private readonly string playerPath;
        private readonly string enemyPath;
        private readonly string collectiblePath;
        private readonly string dataPath;
        private readonly string hudPath;
        private readonly string statPath;
        private const string nullString = "null";
        private const char slash = '/';

        private Map Map;
        public static Camera Camera { get; private set; }
        private List<Sprite> Sprites;
        private CollisionManager collisionManager;
        private Dictionary<string, Tile> tileTemplates;
        private List<Weapon> weaponTemplates;
        private List<Collectible> collectibleTemplates;
        private List<Enemy> enemyTemplates;
        private Player Player;

        private int noOfWeapons;
        private int desiredNoOfWeapons;
        private int noOfCollectibles;
        private int desiredNoOfCollectibles;
        private int enemyCount;
        private int desiredNoOfEnemies;
        private float enclosedTimer;
        private float enclosedInterval;
        private float gameDuration;
        private int allowedRadius;

        private Vector2 positionHUD;
        private int spacingHUD;

        private ContentManagerPlus contentManager;

        public World(string TilePath, string MapPath, string WeaponPath, string PlayerPath, string EnemyPath, string CollectiblePath, string DataPath, string HUDPath, string StatPath, ContentManagerPlus content)
        {
            //sets the necessary file paths so all the data can be loaded into the game
            tilePath = TilePath;
            mapPath = MapPath;
            weaponPath = WeaponPath;
            playerPath = PlayerPath;
            enemyPath = EnemyPath;
            collectiblePath = CollectiblePath;
            dataPath = DataPath;
            hudPath = HUDPath;
            statPath = StatPath;
            contentManager = content;
        }
        public void Initialise()
        {
            //Initialise method, effectively loading and generating everything needed in order to play the game
            LoadTileInfo();
            LoadMap();
            LoadWeapons();
            LoadCollectibles();
            LoadEnemyInfo();
            LoadMetaData();
            Camera = new Camera(Game1.ScreenWidth, Game1.ScreenHeight);
            Sprites = new List<Sprite>();
            collisionManager = new CollisionManager();
            LoadAllTiles();
            GeneratePlayer();
            GenerateEnemies();
            GenerateWeapons();
            GenerateCollectibles();
        }

        private void LoadMetaData()
        {
            try
            {
                //load extra data needed for game to run
                string[] file = File.ReadAllLines(dataPath);
                string[] split = file[0].Split(",");
                desiredNoOfCollectibles = int.Parse(split[0]);
                desiredNoOfWeapons = int.Parse(split[1]);
                desiredNoOfEnemies = int.Parse(split[2]);
                positionHUD = new Vector2(int.Parse(split[3]), int.Parse(split[4]));
                spacingHUD = int.Parse(split[5]);
                allowedRadius = int.Parse(split[6]);
                enclosedInterval = float.Parse(split[7]);
            }
            catch (Exception ex)
            {
                Game1.gameState = GameState.Error;
                Game1.errorText = ex.Message;
            }


        }
        private void LoadTileInfo()
        {
            try
            {
                //load tile templates used for getting correct tile in the loading of the map
                string[] file = File.ReadAllLines(tilePath);
                tileSize = int.Parse(file[0]);
                List<Tile> tiletypes = new List<Tile>();
                for (int i = 1; i < file.Length; i++)
                {
                    if (file[i] != "#")
                    {
                        string current = file[i];
                        string[] currentSplit = current.Split(',');
                        //create tile using characteristics of current line in the file
                        //format is tileID, name of texture, whether tile is solid or not
                        Tile currentTile = new Tile(currentSplit[0], currentSplit[1], bool.Parse(currentSplit[2]));
                        tiletypes.Add(currentTile);
                    }
                    else
                    {
                        break;
                    }
                }
                //add tiles to the template list
                tileTemplates = new Dictionary<string, Tile>();
                for (int i = 0; i < tiletypes.Count; i++)
                {
                    //use tileID as key of the dictionary
                    tileTemplates.Add(tiletypes[i].tileID, tiletypes[i]);
                }
            }
            catch (Exception ex)
            {
                Game1.gameState = GameState.Error;
                Game1.errorText = ex.Message;
            }



        }

        private void LoadMap()
        {
            try
            {
                //create a new map and give it a width and height using the size of the lines and number of lines in the file
                Map = new Map();
                string[] mapfile = File.ReadAllLines(mapPath);
                //width is size of a line
                int horizontalSize = mapfile[0].Split(',').Length;
                //height is number of lines in the text file
                int verticalSize = mapfile.Length;
                Map.mapHeight = verticalSize;
                Map.mapWidth = horizontalSize;
                Map.tileMap = new Tile[Map.mapHeight, Map.mapWidth];
                Map.tileSize = tileSize;
                Map.CreateEnclosedTexture();
                for (int i = 0; i < Map.mapHeight; i++)
                {
                    for (int j = 0; j < Map.mapWidth; j++)
                    {
                        //give current tile in the tilemap the correct texture and characteristics based on its ID
                        var currentTile = tileTemplates[mapfile[i].Split(',')[j]];
                        Map.tileMap[i, j] = new Tile(currentTile.tileID, new Point(i, j), currentTile.textureName, currentTile.isSolid);
                    }
                }

            }
            catch (Exception ex)
            {
                Game1.gameState = GameState.Error;
                Game1.errorText = ex.Message;
            }

        }

        private void LoadWeapons()
        {
            try
            {
                //load weapons from file and create a list of cloneable weapons
                //that can be used to dynamically create weapon objects
                weaponTemplates = new List<Weapon>();
                string[] file = File.ReadAllLines(weaponPath);
                for (int i = 0; i < file.Length; i++)
                {
                    if (file[i] != "#")
                    {
                        string[] split = file[i].Split(",");
                        Weapon currentWeapon = new Weapon(
                            new Vector2(0, 0),
                            (WeaponType)int.Parse(split[0]),
                            split[1],
                            float.Parse(split[2]),
                            float.Parse(split[3]),
                            float.Parse(split[4]),
                            split[5],
                            float.Parse(split[6]),
                            float.Parse(split[7]),
                            null
                            );
                        weaponTemplates.Add(currentWeapon);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Game1.gameState = GameState.Error;
                Game1.errorText = ex.Message;
            }
        }

        private void LoadCollectibles()
        {
            try
            {
                //load collectibles from file and create a list of cloneable collectibles
                //that can be used to dynamically create collectible objects
                collectibleTemplates = new List<Collectible>();
                string[] file = File.ReadAllLines(collectiblePath);
                for (int i = 0; i < file.Length; i++)
                {
                    if (file[i] != "#")
                    {
                        string[] split = file[i].Split(",");
                        Collectible currentCollectible = new Collectible(Vector2.Zero, split[0], int.Parse(split[1]));
                        collectibleTemplates.Add(currentCollectible);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Game1.gameState = GameState.Error;
                Game1.errorText = ex.Message;
            }


        }

        private void LoadEnemyInfo()
        {
            try
            {
                //load enemies from file and create a list of cloneable enemies
                //that can be used to dynamically create enemies
                enemyTemplates = new List<Enemy>();
                string[] file = File.ReadAllLines(enemyPath);
                for (int i = 0; i < file.Length; i++)
                {
                    if (file[i] != "#")
                    {
                        string[] split = file[i].Split(",");
                        Enemy currentEnemy = new Enemy
                            (Vector2.Zero,
                            split[0],
                            float.Parse(split[1]),
                            float.Parse(split[2]),
                            (WeaponBias)int.Parse(split[3]),
                            int.Parse(split[4]),
                            float.Parse(split[5]),
                            float.Parse(split[6]));
                        enemyTemplates.Add(currentEnemy);
                    }
                    else
                    {
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                Game1.gameState = GameState.Error;
                Game1.errorText = ex.Message;
            }
        }

        private void LoadAllTiles()
        {
            //load all tile textures in the map
            try
            {
                Map.LoadTileTextures(contentManager);
            }
            catch (Exception ex)
            {
                Game1.gameState = GameState.Error;
                Game1.errorText = ex.Message;
            }
        }

        private void GeneratePlayer()
        {
            try
            {
                //create a player using the characteristics from the file and adds it to the sprites list
                string[] file = File.ReadAllLines(playerPath);
                var split = file[0].Split(",");
                //give a random starting position to the player
                Vector2 position = Map.WorldToScreen(Map.GetRandomValidSquare());
                Player = new Player(position, split[0], float.Parse(split[1]), float.Parse(split[2]));
                Player.LoadTexture(contentManager);
                Sprites.Add(Player);
            }
            catch (Exception ex)
            {
                Game1.gameState = GameState.Error;
                Game1.errorText = ex.Message;
            }
        }

        private void GenerateEnemies()
        {
            //generates enemies in random positions in the map
            //selects a random enemy from a list of templates and adds it to the world
            //each enemy template has a different texture and weapon bias
            Random r = new Random();
            while (enemyCount < desiredNoOfEnemies)
            {
                Point pickedPoint = Map.GetRandomValidSquare();
                //if there is another agent in a predefined radius of the picked position then regenerate the position until there isnt
                while (GetSpritesInRange(pickedPoint, allowedRadius, allowedRadius, Sprites, true).Count != 0)
                {
                    pickedPoint = Map.GetRandomValidSquare();
                }
                Enemy newEnemy = (Enemy)enemyTemplates[r.Next(0, enemyTemplates.Count)].Clone();
                newEnemy.Position = Map.WorldToScreen(pickedPoint);
                newEnemy.LoadTexture(contentManager);
                Sprites.Add(newEnemy);
                enemyCount++;
            }
        }

        private void GenerateWeapons()
        {
            //generates weapons in random positions in the map
            //selects a random weapon from a list of templates and adds it to the world
            Random r = new Random();
            while (noOfWeapons < desiredNoOfWeapons)
            {
                Point pickedPoint = Map.GetRandomValidSquare();
                Weapon newWeapon = (Weapon)weaponTemplates[r.Next(0, weaponTemplates.Count)].Clone();
                newWeapon.Position = Map.WorldToScreen(pickedPoint);
                newWeapon.LoadTexture(contentManager);
                Sprites.Add(newWeapon);
                noOfWeapons++;
            }
        }

        private void GenerateCollectibles()
        {
            //generates collectibles in random positions in the map
            //selects a random collectible from a list of templates and adds it to the world
            Random r = new Random();
            while (noOfCollectibles < desiredNoOfCollectibles)
            {
                Point pickedPoint = Map.GetRandomValidSquare();
                Collectible newCollectible = (Collectible)collectibleTemplates[r.Next(0, collectibleTemplates.Count)].Clone();
                newCollectible.Position = Map.WorldToScreen(pickedPoint);
                newCollectible.LoadTexture(contentManager);
                Sprites.Add(newCollectible);
                noOfCollectibles++;
            }
        }

        public static List<Sprite> GetSpritesInRange(Point pointForChecking, int radiusX, int radiusY, List<Sprite> Sprites, bool AgentsOnly)
        {
            //gets every sprite that is in a certain radius of a position
            //only returns agent sprites if AgentsOnly is true
            Point upperTile = Point.Plus(pointForChecking, new Point(radiusX, radiusY));
            Point lowerTile = Point.Plus(pointForChecking, new Point(-radiusX, -radiusY));
            return Sprites.Where(s =>
            (s.worldPosition.X >= lowerTile.X) &&
            (s.worldPosition.Y >= lowerTile.Y) &&
            (s.worldPosition.X <= upperTile.X) &&
            (s.worldPosition.Y <= upperTile.Y) &&
            (AgentsOnly == false || (AgentsOnly == true && s is Agent))
            ).ToList();
        }

        public void Update(GameTime gameTime)
        {
            try
            {
                //update time since last enclosing of the map has happened
                gameDuration += (float)gameTime.ElapsedGameTime.TotalSeconds;
                enclosedTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (enclosedTimer > enclosedInterval)
                {
                    //make map smaller if interval has passed
                    Map.EncloseMap();
                    enclosedTimer = 0;
                }
                //when collectibles or weapons are removed, regenerate them so there is always a certain number in the game
                GenerateCollectibles();
                GenerateWeapons();
                for (int i = 0; i < Sprites.Count; i++)
                {
                    if (Sprites[i] is Enemy)
                    {
                        //gets all the sprites the current enemy being updated can see
                        List<Sprite> spritesInRangeForEnemy = GetSpritesInRange(Sprites[i].worldPosition,
                            ((Enemy)Sprites[i]).tileVision,
                            ((Enemy)Sprites[i]).tileVision,
                            Sprites,
                            false);
                        //map is sent so enemy can pathfind if needed
                        ((Enemy)Sprites[i]).Update(gameTime, spritesInRangeForEnemy, Map);
                    }
                    else
                    {
                        Sprites[i].Update(gameTime);
                    }

                    if (Sprites[i] is Agent)
                    {
                        UpdateAgent((Agent)Sprites[i]);
                    }
                }
                collisionManager.Update(gameTime, Map, Sprites);
                DropDeadWeapons();
                enemyCount = Sprites.Where(s => s is Enemy).ToList().Count;
                noOfWeapons = Sprites.Where(s => s is Weapon).ToList().Count;
                Camera.Update(Player, Map);
                if (Sprites.Where(s => s is Agent).ToList().Count == 1 || Player.isRemoved == true)
                {
                    Game1.gameState = GameState.GameOver;
                }
            }
            catch (Exception Ex)
            {
                Game1.gameState = GameState.Error;
                Game1.errorText = Ex.Message;
            }

        }

        private void UpdateAgent(Agent agent)
        {
            //Checks for what the agent wants to do and does it
            if (IsAgentShooting(agent) == true)
            {
                ShootForAgent(agent);
            }
            if (IsAgentPickingUp(agent) == true && agent.ReadyToPick())
            {
                PickUpWeaponForAgent(agent);
            }
            PickUpCollectible(agent);
        }

        private bool IsAgentShooting(Agent shootingagent)
        {
            return shootingagent.IsShooting;
        }
        private void ShootForAgent(Agent shootingagent)
        {
            if (shootingagent.equippedWeapon == null)
            {
                //if agent does not have a weapon they cannot shoot so return
                return;
            }
            else
            {
                if (shootingagent.equippedWeapon.ReadyToShoot())
                {
                    //creates a projectile from the agents position in the direction the agent is shooting
                    Projectile projectileShot = shootingagent.equippedWeapon.GetCloneOfBaseProjectile();
                    projectileShot.Position = shootingagent.Position;
                    projectileShot.Direction = shootingagent.projectileDirection;
                    projectileShot.Parent = shootingagent;
                    //loads texture so projectile can be drawn
                    projectileShot.LoadTexture(contentManager);
                    projectileShot.SetVelocity();
                    //added to the World's list of sprites so the projectile is updated and drawn every frame
                    Sprites.Add(projectileShot);
                    //reset the time since last shot of the agents weapon
                    shootingagent.equippedWeapon.ResetTime();
                }

            }

        }

        private bool IsAgentPickingUp(Agent targetAgent)
        {
            return targetAgent.IsPickingUp;
        }

        private void PickUpWeaponForAgent(Agent holdingagent)
        {
            Weapon droppedWeapon = holdingagent.equippedWeapon;
            if (holdingagent.equippedWeapon != null)
            {
                //if player is picking up they are also dropping their current weapon
                DropWeapon(holdingagent.equippedWeapon, holdingagent);
            }
            for (int i = 0; i < Sprites.Count; i++)
            {
                if (Sprites[i] is Weapon)
                {
                    //ensure weapon is not the weapon that has just been dropped
                    //ensure weapon is not equipped by another agent
                    //only check if they intersect if it is a viable weapon to be picked up
                    if (((Weapon)Sprites[i]).Holder == null && (Weapon)Sprites[i] != droppedWeapon)
                    {
                        //check if weapon intersects with current sprite                      
                        if (((Weapon)Sprites[i]).Rectangle.Intersects(holdingagent.Rectangle))
                        {
                            //set agents weapon to weapon they are picking up
                            holdingagent.UpdateWeapon((Weapon)Sprites[i]);
                            //set weapons current owner to the agent that picked it up
                            ((Weapon)Sprites[i]).UpdateHolder(holdingagent);
                            holdingagent.ResetTimeSinceLastPicked();
                            //stop if a weapon has been picked up
                            //ensures weapon does not pick up more than one weapon if the agent intersects with multiple
                            return;
                        }
                    }
                }
            }
            holdingagent.ResetTimeSinceLastPicked();
        }

        private void DropDeadWeapons()
        {
            //when an agent dies, drop their weapon
            List<Sprite> weapons = Sprites.Where(s => s is Weapon).ToList();
            foreach (Weapon w in weapons)
            {
                if (w.Holder != null)
                {
                    if (w.Holder.isRemoved == true)
                    {
                        DropWeapon(w, w.Holder);
                    }
                }
            }
        }
        private void DropWeapon(Weapon weapon, Agent agent)
        {
            //Drops the weapon from the agent holding it, the weapon can now be picked up and the agent will no longer have this weapon equipped
            weapon.UpdateHolder(null);
            agent.UpdateWeapon(null);
        }

        private void PickUpCollectible(Agent pickingAgent)
        {
            List<Sprite> collectibles = Sprites.Where(s => s is Collectible).ToList();
            //goes through all the collectibles and checks if any of them intersect with the current agent
            //if they do then the agent will collect it and the collectible will be removed

            foreach (Collectible collectible in collectibles.Cast<Collectible>())
            {
                if (collectible.Rectangle.Intersects(pickingAgent.Rectangle))
                {
                    pickingAgent.CollectHealth(collectible);
                    collectible.isRemoved = true;
                    noOfCollectibles--;
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            //draw world using centred on camera position
            spriteBatch.Begin(transformMatrix: Camera.Transform);
            Map.Draw(spriteBatch);
            Player.Draw(spriteBatch);
            DrawSpritesInRange(spriteBatch);
            spriteBatch.End();
            //start a separate spritebatch as HUD is not affected by camera
            spriteBatch.Begin();
            DrawHUD(spriteBatch);
        }

        private void DrawSpritesInRange(SpriteBatch spriteBatch)
        {
            //only draws sprites that are visible to the player based on their position
            //avoids unnecessary drawing of sprites
            Point radius = Map.ScreenToWorld(new Vector2(Game1.ScreenWidth, Game1.ScreenHeight));
            List<Sprite> inRange = GetSpritesInRange(Player.worldPosition, radius.X, radius.Y, Sprites, false);

            foreach (Sprite s in inRange)
            {
                if (s is Player == false)
                {
                    s.Draw(spriteBatch);
                }
            }
        }

        private void DrawHUD(SpriteBatch spriteBatch)
        {
            //Display relevant information for the player
            string[] file = File.ReadAllLines(hudPath);
            List<string> HUDtext = new List<string>()
            {
                file[0] + ": " + Player.Health.ToString() + slash + Player.maxHealth.ToString(),
                file[1] + ": " + ((Player.equippedWeapon != null) ? Player.equippedWeapon.weaponType.ToString() : nullString),
                file[2] + ": " + ((Player.equippedWeapon != null) ? Player.equippedWeapon.TimeLeftTillShoot().ToString() : nullString),
                file[3] + ": " + (enemyCount + 1).ToString(),
                file[4] + ": " + Player.Kills.ToString(),
                file[5] + ": " + ((int)(enclosedInterval - enclosedTimer)).ToString(),
                file[6]
            };
            Vector2 pos = new Vector2(positionHUD.X, positionHUD.Y);
            for (int i = 0; i < HUDtext.Count; i++)
            {
                spriteBatch.DrawString(Game1.Font, HUDtext[i], pos, Color.White);
                pos.Y += spacingHUD;
            }
        }

        public bool PlayerWin()
        {
            //returns true is player has won the game and false if player has lost
            return Player.isRemoved == false && Sprites.Where(s => s is Agent).ToList().Count == 1;
        }

        public List<string> GetPlayerStats()
        {
            try
            {
                //returns list of statistics used to display when game has finished
                int placeFinished = Sprites.Where(s => s is Agent).ToList().Count();
                if (Player.isRemoved == true)
                {
                    placeFinished += 1;
                }               
                string[] file = File.ReadAllLines(statPath);
                return new List<string>()
                {
                file[0] + ": " + placeFinished.ToString(),
                file[1] + ": " + Player.Kills.ToString(),
                file[2] + ": " + Player.damageTaken.ToString(),
                file[3] + ": " + ((int)gameDuration).ToString()
                };
            }
            catch (Exception Ex)
            {
                Game1.gameState = GameState.Error;
                Game1.errorText = Ex.Message;
                return null;
            }
            

        }
        
    }
}

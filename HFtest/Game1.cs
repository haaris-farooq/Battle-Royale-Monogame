using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using HFtest;
using System.Linq.Expressions;

namespace ComputingProjectHF
{
    public class Game1 : Game
    {
        //constants, numbers that are used frequently and list of files needed for game to run
        private const int itemSpacing = 15;
        private const float third = 0.3f;
        private const float half = 0.5f;
        private const string tilePath = "TileInfo.txt";
        private const string mapPath = "map1.txt";
        private const string weaponPath = "Weapons.txt";
        private const string playerPath = "Player.txt";
        private const string enemyPath = "Enemy.txt";
        private const string collectiblePath = "Collectibles.txt";
        private const string dataPath = "Metadata.txt";
        private const string hudPath = "HUD.txt";
        private const string statPath = "Stats.txt";
        private const string infoPath = "Info.txt";
        private const string menuPath = "MenuItems.txt";
        private const string pausePath = "PauseItems.txt";
        private const string loadingText = "Loading...";
        private const string victoryText = "Victory";
        private const string defeatText = "Defeat";
        private Color menuColour = Color.Violet;
        private Color winColour = Color.Green;
        private Color defeatColour = Color.Red;
        private Color loadingColour = Color.CornflowerBlue;
        private Color errorColour = Color.DarkRed;

        private static GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static int ScreenWidth;
        public static int ScreenHeight;
        public static SpriteFont Font;
        public static string debugText = "";
        public static string errorText = "";
        public static GameState gameState;

        private World World;
        private MenuScreen Menu;
        private MenuScreen Pause;
        private GameScreen gameOver;
        private GameScreen Info;
        private ContentManagerPlus contentManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Sets the heights and width to the size of the window
            ScreenHeight = _graphics.PreferredBackBufferHeight;
            ScreenWidth = _graphics.PreferredBackBufferWidth;
            contentManager = new ContentManagerPlus(Content.ServiceProvider, Content.RootDirectory);
            gameOver = null;
            Info = null;
            Pause = null;
            gameState = GameState.Menu;
            base.Initialize();


        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //load font for drawing strings
            Font = Content.Load<SpriteFont>("Font");

        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Main loop
            //update current key board state

            KeyboardManager.Update();
            switch (gameState)
            {
                case GameState.Loading:
                    //if we are loading then create a new world
                    World = new World(tilePath, mapPath, weaponPath, playerPath, enemyPath, collectiblePath, dataPath, hudPath, statPath, contentManager);
                    World.Initialise();
                    //if we have finished loading then we can start playing
                    if (gameState != GameState.Error)
                    {
                        gameState = GameState.Playing;
                    }
                    break;
                case GameState.Menu:
                    if (Menu == null)
                    {
                        //if we dont have a menu screen then create it
                        CreateMenuScreen();
                    }
                    Menu.Update(gameTime);
                    if (KeyboardManager.IsKeyPressedOnce(Keys.Enter))
                    {
                        switch (Menu.GetSelectedItem())
                        {
                            //first item = Play
                            case 0:
                                Menu = null;
                                gameState = GameState.Loading;
                                break;
                            //second item = Info screen
                            case 1:
                                gameState = GameState.Info;
                                break;
                            //third item = Quit
                            case 2:
                                Environment.Exit(0);
                                break;
                        }

                    }
                    break;
                case GameState.Playing:
                    World.Update(gameTime);
                    if (KeyboardManager.IsKeyPressedOnce(Keys.P))
                    {
                        //pause game is key is pressed
                        gameState = GameState.Pause;
                    }
                    break;
                case GameState.Pause:
                    if (Pause == null)
                    {
                        CreatePauseScreen();
                    }
                    Pause.Update(gameTime);
                    if (KeyboardManager.IsKeyPressedOnce(Keys.Enter))
                    {
                        switch (Pause.GetSelectedItem())
                        {
                            //first item is return to game
                            case 0:
                                gameState = GameState.Playing;
                                break;
                            //second item is return to menu
                            case 1:
                                gameState = GameState.Menu;
                                break;
                        }
                    }
                    break;
                case GameState.GameOver:
                    if (gameOver == null)
                    {
                        CreateGameOverScreen();
                    }
                    if (KeyboardManager.IsKeyPressedOnce(Keys.Enter))
                    {
                        //return to menu if user has read game over screen
                        gameOver = null;
                        gameState = GameState.Menu;
                    }
                    break;
                case GameState.Info:
                    if (KeyboardManager.IsKeyPressedOnce(Keys.Enter))
                    {
                        //return to menu if user has read info screen
                        gameState = GameState.Menu;
                    }
                    break;
                case GameState.Error:
                    if (KeyboardManager.IsKeyPressedOnce(Keys.Enter))
                    {
                        //exit game if error but not after error message is displayed
                        Environment.Exit(0);
                    }
                    break;
            }
            KeyboardManager.UpdatePrevious();
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            Vector2 centralTextPos = new Vector2(ScreenWidth * half, ScreenHeight * half);
            // TODO: Add your drawing code here
            try
            {
                switch (gameState)
                {
                    case GameState.Loading:
                        //display loading screen if game is loading
                        GraphicsDevice.Clear(Color.CornflowerBlue);
                        spriteBatch.Begin();
                        spriteBatch.DrawString(Font, loadingText, centralTextPos, Color.White);
                        break;
                    case GameState.Menu:
                        if (Menu == null)
                        {
                            CreateMenuScreen();
                        }
                        spriteBatch.Begin();
                        Menu.Draw(spriteBatch);
                        break;
                    case GameState.Playing:
                        World.Draw(spriteBatch);
                        break;
                    case GameState.Pause:
                        if (Pause == null)
                        {
                            //if pause screen doesnt exist then create it
                            CreatePauseScreen();
                        }
                        spriteBatch.Begin();
                        Pause.Draw(spriteBatch);
                        break;
                    case GameState.Info:
                        if (Info == null)
                        {
                            //if info screen doesnt exist then create it
                            CreateInfoScreen();
                        }
                        spriteBatch.Begin();
                        Info.Draw(spriteBatch);
                        break;
                    case GameState.GameOver:
                        if (gameOver == null)
                        {
                            //if ending screen doesnt exist then create it
                            CreateGameOverScreen();
                        }
                        spriteBatch.Begin();
                        gameOver.Draw(spriteBatch);
                        break;
                    case GameState.Error:
                        //display error message
                        GraphicsDevice.Clear(errorColour);
                        spriteBatch.Begin();
                        spriteBatch.DrawString(Font, errorText, centralTextPos, Color.White);
                        break;
                }
                spriteBatch.End();
                base.Draw(gameTime);
            }
            catch (Exception Ex)
            {
                gameState = GameState.Error;
                errorText = Ex.Message;
            }
        }

        private void CreateMenuScreen()
        {
            //creates menu screen using text items from a file
            Texture2D screenTexture = GenerateTexture(ScreenWidth, ScreenHeight, menuColour);
            string[] file = File.ReadAllLines(menuPath);
            List<string> items = file.ToList();
            Menu = new MenuScreen(screenTexture, items, new Vector2(ScreenWidth * half, ScreenHeight * third), itemSpacing, Font);
        }

        private void CreatePauseScreen()
        {
            //create a menu screen for when the game has paused
            try
            {
                Texture2D screenTexture = GenerateTexture(ScreenWidth, ScreenHeight, menuColour);
                string[] file = File.ReadAllLines(pausePath);
                List<string> items = file.ToList();
                Pause = new MenuScreen(screenTexture, items, new Vector2(ScreenWidth * half, ScreenHeight * third), itemSpacing, Font);
            }
            catch (Exception Ex)
            {
                gameState = GameState.Error;
                errorText = Ex.Message;
            }


        }

        private void CreateGameOverScreen()
        {
            //create a screen that displays the result of the game and the stats for that game
            try
            {
                bool victory = World.PlayerWin();
                Texture2D screenTexture = GenerateTexture(ScreenWidth, ScreenHeight, victory ? winColour : defeatColour);
                List<string> items = new List<string>()
                { 
                    //if player has won then display victory text and if not then display defeat text
                    victory ? victoryText : defeatText
                };
                items.AddRange(World.GetPlayerStats());
                gameOver = new GameScreen(screenTexture, items, new Vector2(ScreenWidth * half, ScreenHeight * third), itemSpacing, Font);
            }
            catch (Exception Ex)
            {
                gameState = GameState.Error;
                errorText = Ex.Message;
            }

        }

        private void CreateInfoScreen()
        {
            //Create a screen for displaying info on the game, all items are read from a file containing game info
            try
            {
                string[] info = File.ReadAllLines(infoPath);
                List<string> items = info.ToList();
                Texture2D screenTexture = GenerateTexture(ScreenWidth, ScreenHeight, menuColour);
                Info = new GameScreen(screenTexture, items, new Vector2(ScreenWidth * third, ScreenHeight * third), itemSpacing, Font);
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                gameState = GameState.Error;
            }
        }



        //From Teacher
        public static Texture2D GenerateTexture(int width, int height, Color colour) //create a texture if you don't want to load any
        {
            Texture2D texture = new Texture2D(_graphics.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Length; pixel++)
            {
                data[pixel] = colour;
            }
            texture.SetData(data);
            return texture;
        }
    }
}

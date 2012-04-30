#region File Description
//-----------------------------------------------------------------------------
// PlatformerGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Eve
{
    /// <summary>
    /// This is the main class of our game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        public static int[] totalStages = { 2, 1 };
        
        /// <summary>
        /// Create a new game.
        /// </summary>
        public Game()
        {
            // initialize the graphics system
            graphics = new GraphicsDeviceManager(this);
            
            // configure the content manager
            Content.RootDirectory = "Content";

            // add a gamer-services component, which is required for the storage APIs
            Components.Add(new GamerServicesComponent(this));

            // add the screen manager
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to 
        /// before starting to run.  This is where it can query for any required 
        /// services and load any non-graphic related content.  Calling base.Initialize 
        /// will enumerate through any components and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InputManager.Initialize();

            FactoidManager.Initialize();

            ObjectManager.Initialize();

            base.Initialize();

            screenManager.AddScreen(new MainMenuScreen());
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of our content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            InputManager.Update();

            base.Update(gameTime);
        }


        #region Drawing


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.White);

            base.Draw(gameTime);
        }


        #endregion


        #region Main

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Run();
            }
        }

        #endregion
    }
}

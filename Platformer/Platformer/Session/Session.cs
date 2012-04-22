#region File Description
//-----------------------------------------------------------------------------
// Session.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Eve
{
    class Session
    {
        #region Singleton


        /// <summary>
        /// The single Session instance that can be active at a time.
        /// </summary>
        private static Session singleton;


        #endregion


        #region Level


        /// <summary>
        /// The level that the current session is in.
        /// </summary>
        private Level level;


        /// <summary>
        /// Reload the level from the last checkpoint.
        /// </summary>
        public static void ReloadLevel()
        {
            Level.StartNewLife();
        }


        /// <summary>
        /// The level that the current session is in.
        /// </summary>
        public static Level Level
        {
            get { return (singleton == null ? null : singleton.level); }
            set { singleton.level = value; }
        }

        
        /// <summary>
        /// Load the level at based on the information from the stats in the current session.
        /// </summary>
        public static void LoadLevel()
        {
            Level = new Level(ScreenManager.Game.Content, ScreenManager.GraphicsDevice.Viewport);
        }


        #endregion


        #region User Interface Data


        /// <summary>
        /// The ScreenManager used to manage all UI in the game.
        /// </summary>
        private ScreenManager screenManager;

        /// <summary>
        /// The ScreenManager used to manage all UI in the game.
        /// </summary>
        public static ScreenManager ScreenManager
        {
            get { return (singleton == null ? null : singleton.screenManager); }
        }


        /// <summary>
        /// The GameplayScreen object that created this session.
        /// </summary>
        private GameplayScreen gameplayScreen;


        /// <summary>
        /// The GameplayScreen object that created this session.
        /// </summary>
        public static GameplayScreen GameplayScreen
        {
            get { return (singleton == null ? null : singleton.gameplayScreen); }
        }


        /// <summary>
        /// The current stats of the current session.
        /// </summary>
        private StatisticsManager statisticsManager;


        /// <summary>
        /// The current stats of the current session.
        /// </summary>
        public static StatisticsManager StatisticsManager
        {
            get { return singleton == null ? null : singleton.statisticsManager; }
        }


        /// <summary>
        /// Stats from the last major checkpoint.
        /// </summary>
        private StatisticsManager lastSavedStats;


        /// <summary>
        /// The current stats of the current session.
        /// </summary>
        public static StatisticsManager LastSavedStats
        {
            get { return singleton == null ? null : singleton.lastSavedStats; }
            set { singleton.lastSavedStats = value; }
        }


        /// <summary>
        /// The HUD of the current session.
        /// </summary>
        private HUD hud;


        /// <summary>
        /// The HUD of the current session.
        /// </summary>
        public static HUD HUD
        {
            get { return singleton.hud; }
            set { singleton.hud = value; }
        }


        #endregion


        #region State Data


        /// <summary>
        /// Returns true if there is an active session.
        /// </summary>
        public static bool IsActive
        {
            get { return singleton != null; }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Private constructor of a Session object.
        /// </summary>
        private Session(ScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameter
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (gameplayScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // assign the parameter
            this.screenManager = screenManager;
            this.gameplayScreen = gameplayScreen;
        }


        #endregion


        #region Updating


        /// <summary>
        /// Update the session for this frame.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            // check the singleton
            if (singleton == null)
            {
                return;
            }
            
            StatisticsManager.SetPosition(Level.Player.Position);
            StatisticsManager.UpdateEnemies(Level.enemies);
            StatisticsManager.UpdateObjects(Level.objects);
            Level.Update(gameTime);
            // Time should only be updated when the gameplay screen is active.
            if (GameplayScreen.IsActive)
            {
                StatisticsManager.IncreaseTotalTime(gameTime.ElapsedGameTime);
                HUD.Update(StatisticsManager.TotalTime);
            }
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draws the session environment to the screen
        /// </summary>
        public static void Draw(GameTime gameTime, Color color, bool freeze = false)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            Level.Draw(gameTime, spriteBatch, color, freeze);
            HUD.Draw(gameTime, spriteBatch, color);
            spriteBatch.End();
        }


        #endregion


        #region Starting a Session


        /// <summary>
        /// Start a new session based on the data provided.
        /// </summary>
        public static void StartSession(StatisticsManager statisticsManager, 
            ScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameters
            if (statisticsManager == null)
            {
                throw new ArgumentNullException("statisticsManager");
            }
            if (statisticsManager.LevelIndex < 0 || statisticsManager.LevelIndex >= Game.totalLevels)
            {
                throw new ArgumentNullException("levelIndex");
            }
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (gameplayScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // end any existing session
            EndSession();

            // create a new singleton
            singleton = new Session(screenManager, gameplayScreen);

            // load the singleton's stats with the provided stats
            singleton.statisticsManager = statisticsManager;
            singleton.lastSavedStats = new StatisticsManager(statisticsManager);

            // set up the initial level
            LoadLevel();

            // set up the HUD of the game.
            HUD = new HUD();
        }


        #endregion


        #region Ending a Session


        /// <summary>
        /// End the current session.
        /// </summary>
        public static void EndSession()
        {
            // exit the gameplay screen
            // -- store the gameplay session, for re-entrance
            if (singleton != null)
            {
                GameplayScreen gameplayScreen = singleton.gameplayScreen;
                singleton.gameplayScreen = null;

                // clear the singleton
                singleton = null;

                if (gameplayScreen != null)
                {
                    gameplayScreen.ExitScreen();
                }
            }
        }


        #endregion


        #region Random


        /// <summary>
        /// The random-number generator used with game events.
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// The random-number generator used with game events.
        /// </summary>
        public static Random Random
        {
            get { return random; }
        }


        #endregion
    }
}

#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Eve
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields


        /// <summary>
        /// Parameter that can freeze the current session.
        /// </summary>
        private bool freeze = false;


        /// <summary>
        /// Parameter that can freeze the current session.
        /// </summary>
        public bool Freeze
        {
            get { return freeze; }
            set { freeze = value; }
        }


        /// <summary>
        /// The current save manager of the game.
        /// </summary>
        private SaveManager saveManager;


        /// <summary>
        /// The current save manager of the game.
        /// </summary>
        public SaveManager SaveManager
        {
            get { return saveManager; }
        }


        #endregion


        #region Events


        /// <summary>
        /// A delegate type for hooking up checkpoint notifications.
        /// </summary>
        private delegate void CheckpointEventHandler(object sender, EventArgs e, Sign sign);


        /// <summary>
        /// Event raised when a checkpoint is reached.
        /// </summary>
        private event CheckpointEventHandler Checkpoint;


        /// <summary>
        /// Method for raising the checkpoint event.
        /// </summary>
        public void CheckpointReached(Sign sign)
        {
            if (Checkpoint != null)
                Checkpoint(this, EventArgs.Empty, sign);
        }


        /// <summary>
        /// Event handler for when a checkpoint is reached. The game is saved everytime 
        /// a checkpoint is reached.
        /// </summary>
        private void CheckpointReached(object sender, EventArgs e, Sign sign)
        {
            SaveManager.SetStatistics(Session.StatisticsManager);
            Session.LastSavedStats = new StatisticsManager(Session.StatisticsManager);
            SaveManager.SaveData();
            freeze = true;
            ScreenManager.AddScreen(new SignScreen(sign));
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Create a new GameplayScreen object.
        /// </summary>
        private GameplayScreen() : base()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            Checkpoint += CheckpointReached;
            
            this.Exiting += new EventHandler(GameplayScreen_Exiting);
        }

        
        /// <summary>
        /// Create a new GameplayScreen object from the stats provided.
        /// </summary>
        public GameplayScreen(SaveManager saveManager = null) : this()
        {
            // If save manager does not exist, then new game was selected.
            if (saveManager == null)
            {
                this.saveManager = new SaveManager();
            }
            // Otherwise set the current save manager to the save state given.
            else
            {
                this.saveManager = saveManager;
            }
        }
        

        /// <summary>
        /// Handle the closing of this screen.
        /// </summary>
        void GameplayScreen_Exiting(object sender, EventArgs e)
        {
            // make sure the session is ending
            // -- EndSession must be re-entrant safe, as the EndSession may be 
            //    making this screen close itself
            Session.EndSession();
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            // If save has no statistics from gameplay, then restart from level 0.
            if (saveManager.StatisticsManager == null)
            {
                saveManager.SetStatistics(new StatisticsManager());
            }

            Session.StartSession(saveManager.StatisticsManager, ScreenManager, this);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //Continue to update the session if the screen is active and not covered by a screen or
            //if there is a popup screen that doesn't freeze the game.
            if (IsActive && !coveredByOtherScreen 
                || !IsActive && !freeze) 
            {
                Session.Update(gameTime);
            }


            if (Session.IsActive && IsActive)
            {
                //Continue to next level.
                if (Session.Level.ReachedExit)
                {
                    Session.StatisticsManager.IncreaseLevelIndex();
                    Session.StatisticsManager.ResetPosition();
                    Session.StatisticsManager.UpdateEnemies(null);
                    Session.LastSavedStats = new StatisticsManager(Session.StatisticsManager);
                    SaveManager.SetStatistics(Session.StatisticsManager);
                    ScreenManager.AddScreen(new EndLevelScreen());
                }
                // If the player in the session is dead and the game session is active, bring up the
                // game over screen.
                else if(!Session.Level.Player.IsAlive)
                {
                    // Increment the player's death count in this session.
                    Session.StatisticsManager.IncreaseDeathCount();

                    ScreenManager.AddScreen(new GameOverScreen());
                }
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput()
        {
            if (InputManager.IsActionTriggered(InputManager.Action.Pause))
            {
                freeze = true;
                ScreenManager.AddScreen(new PauseScreen());
                return;
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Color color = new Color(255, 255, 255, TransitionAlpha);
            if (Session.IsActive)
            {
                Session.Draw(gameTime, color, Freeze);
            }
        }


        #endregion
    }
}

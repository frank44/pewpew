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

namespace Platformer
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// </summary>
    class GameplayScreen : GameScreen
    {        
        #region Initialization


        private bool freeze = false;
        public bool Freeze
        {
            get { return freeze; }
            set { freeze = value; }
        }

        private int levelIndex = -1;
        //SaveGameDescription saveGameDescription = null;

        /// <summary>
        /// Create a new GameplayScreen object.
        /// </summary>
        private GameplayScreen() : base()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            this.Exiting += new EventHandler(GameplayScreen_Exiting);
        }


        /// <summary>
        /// Create a new GameplayScreen object from levelIndex.
        /// </summary>
        public GameplayScreen(int levelIndex) : this()
        {
            this.levelIndex = levelIndex;
            //this.saveGameDescription = null;
        }

        /*
        /// <summary>
        /// Create a new GameplayScreen object from a saved-game description.
        /// </summary>
        public GameplayScreen(SaveGameDescription saveGameDescription)
            : this()
        {
            this.gameStartDescription = null;
            this.saveGameDescription = saveGameDescription;
        }
        */

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
            if (levelIndex != -1)
            {
                Session.StartNewSession(levelIndex, ScreenManager, this);
            }
            /*else if (saveGameDescription != null)
            {
                Session.LoadSession(saveGameDescription, ScreenManager, this);
            }*/

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

            //Continue to next level
            if (Session.IsActive && Session.Level.TimeRemaining == TimeSpan.Zero)
            {
                if (Session.Level.ReachedExit)
                {
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen(levelIndex + 1));
                }
                else
                {
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen(levelIndex));
                }
            }

            //If the player in the session is dead and the game session is active, bring up the
            //game over screen.
            if (Session.IsActive && !Session.Level.Player.IsAlive && IsActive)
            {
                ScreenManager.AddScreen(new GameOverScreen());
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

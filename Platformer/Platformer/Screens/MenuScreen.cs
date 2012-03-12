﻿#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Platformer
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        private List<MenuEntry> menuEntries = new List<MenuEntry>();
        protected int selectedEntry = 0;
        private SoundEffect menuMove, selection;
        protected bool preventCancel = false;

        #endregion


        #region Properties

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }


        protected MenuEntry SelectedMenuEntry
        {
            get
            {
                if ((selectedEntry < 0) || (selectedEntry >= menuEntries.Count))
                {
                    return null;
                }
                return menuEntries[selectedEntry];
            }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen() : base()
        { }

        
        /// <summary>
        /// Load the sound effects of this screen.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            menuMove = content.Load<SoundEffect>("Sounds/MenuScreen/MenuMove");
            selection = content.Load<SoundEffect>("Sounds/MenuScreen/Selection");

            base.LoadContent();
        }


        #endregion


        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput()
        {
            int oldSelectedEntry = selectedEntry;

            // Move to the previous menu entry?
            if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                selectedEntry--;
                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                selectedEntry++;
                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            // Accept or cancel the menu?
            if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                selection.Play();
                OnSelectEntry(selectedEntry);
            }
            else if (!preventCancel && (InputManager.IsActionTriggered(InputManager.Action.Back) ||
                InputManager.IsActionTriggered(InputManager.Action.ExitGame)))
            {
                OnCancel();
            }
            else if (selectedEntry != oldSelectedEntry)
            {
                menuMove.Play();
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry();
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];
                
                menuEntry.Draw(this, gameTime);
            }

            spriteBatch.End();
        }


        #endregion
    }
}

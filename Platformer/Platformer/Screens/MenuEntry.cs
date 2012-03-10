#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Platformer
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. The menu entry
    /// is an image of all the entries on the screen but with only the selected 
    /// entry highlighted. See the images in the Sprites folder under the correct 
    /// menu folder to understand. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class MenuEntry
    {
        #region Fields

        /// <summary>
        /// The texture that captures the current menu entry state.
        /// </summary>
        private Texture2D texture;
        
        /// <summary>
        /// The position of this menu item on the screen.
        /// </summary>
        private Vector2 position;
        
        #endregion


        #region Properties

        /// <summary>
        /// The texture that captures the current menu entry state.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        /// <summary>
        /// Gets or sets the position of this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }


        #endregion


        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<EventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        { }


        /// <summary>
        /// Draws the menu entry.
        /// </summary>
        public virtual void Draw(MenuScreen screen, GameTime gameTime)
        {
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            if (texture != null)
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
        }
        
        #endregion
    }
}

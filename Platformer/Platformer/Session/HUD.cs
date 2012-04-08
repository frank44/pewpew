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
    /// <summary>
    /// The HUD tells the player what kind of item they are using and the time elapsed.
    /// </summary>
    class HUD
    {
        #region Properties


        /// <summary>
        /// The HUD for the item currently selected.
        /// </summary>
        private Texture2D itemHUD;


        /// <summary>
        /// The index of what item the player is currently using.
        /// </summary>
        private int index;


        /// <summary>
        /// The total of items that the player can use.
        /// </summary>
        private int total = 3;


        /// <summary>
        /// The current level that the game is in.
        /// </summary>
        private int levelIndex
        {
            get { return Session.StatisticsManager == null ? 0 : Session.StatisticsManager.LevelIndex; }
        }


        /// <summary>
        /// The time that has elapsed in the current level.
        /// </summary>
        private TimeSpan timeElapsed;


        private Vector2 positionItemHUD;
        private Vector2 positionTimeElapsed;
        private SpriteFont hudFont;


        /// <summary>
        /// Offset in both the vertical and horizontal position from the top left corner.
        /// </summary>
        private const int offset = 5;


        #endregion


        #region Events


        /// <summary>
        /// A delegate type for hooking up HUD notifications.
        /// </summary>
        private delegate void HUDEventHandler(object sender, EventArgs e, int direction);


        /// <summary>
        /// Event raised when the HUD needs to update its index of the player's item.
        /// </summary>
        private event HUDEventHandler Switch;


        /// <summary>
        /// Method for raising the HUD update event on the index of the player's item.
        /// </summary>
        public void SwitchWeapon(int direction)
        {
            if (Switch != null)
                Switch(this, EventArgs.Empty, direction);
        }


        /// <summary>
        /// Event handler for when the index of the player's item needs to be switched.
        /// </summary>
        private void SwitchWeapon(object sender, EventArgs e, int direction)
        {
            index = (index+direction+total)%total;
            itemHUD = Session.ScreenManager.Game.Content.Load<Texture2D>(string.Format(@"HUD\hud{0}_{1}", index, levelIndex));
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructs a new HUD.
        /// </summary>
        public HUD()
        {
            index = 0;
            timeElapsed = TimeSpan.Zero;
            positionItemHUD = new Vector2(offset, offset);
            Switch += SwitchWeapon;
            LoadContent();
        }


        /// <summary>
        /// Loads content for the HUD.
        /// </summary>
        public void LoadContent()
        {
            itemHUD = Session.ScreenManager.Game.Content.Load<Texture2D>(string.Format(@"HUD\hud{0}_{1}", index, levelIndex));
            hudFont = Session.ScreenManager.Game.Content.Load<SpriteFont>(@"Fonts\Hud");
            positionTimeElapsed = new Vector2(offset, itemHUD.Height + offset * 2);
        }


        #endregion


        #region Updating and Drawing


        /// <summary>
        /// Updates the HUD with the elapsed game time.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            timeElapsed += gameTime.ElapsedGameTime;
        }


        /// <summary>
        /// Draws the HUD to screen.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(itemHUD, positionItemHUD, color);
            string timeString = "TIME: " + timeElapsed.Minutes.ToString("00") + ":" + timeElapsed.Seconds.ToString("00");
            Color fontColor = Color.Yellow;
            fontColor.A = Session.GameplayScreen.TransitionAlpha;
            spriteBatch.DrawString(hudFont, timeString, positionTimeElapsed, fontColor);
        }


        #endregion
    }
}

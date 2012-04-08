using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Eve
{
    /// <summary>
    /// The confirmation screen gives the options Yes and No.
    /// </summary>
    class ConfirmationScreen : MenuScreen
    {
        #region Fields


        private Texture2D background;
        private Vector2 origin = new Vector2(0, 0);


        #endregion


        #region Menu Entries


        MenuEntry yesMenuEntry, noMenuEntry;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public ConfirmationScreen() : base()
        {
            IsPopup = true;

            yesMenuEntry = new MenuEntry();
            yesMenuEntry.Selected += YesMenuEntrySelected;
            MenuEntries.Add(yesMenuEntry);

            noMenuEntry = new MenuEntry();
            noMenuEntry.Selected += NoMenuEntrySelected;
            MenuEntries.Add(noMenuEntry);
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            background = content.Load<Texture2D>("Backgrounds/PauseScreen/Background");

            // Load textures for the menu.
            yesMenuEntry.Texture = content.Load<Texture2D>("Sprites/Yes");
            yesMenuEntry.Position = new Vector2(viewport.Width - yesMenuEntry.Texture.Width,
                                                viewport.Height - yesMenuEntry.Texture.Height);
            noMenuEntry.Texture = content.Load<Texture2D>("Sprites/No");
            noMenuEntry.Position = new Vector2(viewport.Width - noMenuEntry.Texture.Width,
                                               viewport.Height - noMenuEntry.Texture.Height);

            base.LoadContent();
        }


        #endregion


        #region Updating


        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            base.HandleInput();
        }


        /// <summary>
        /// Event handler for when the Yes menu entry is selected.
        /// </summary>
        protected virtual void YesMenuEntrySelected(object sender, EventArgs e)
        { }


        /// <summary>
        /// Event handler for when the No menu entry is selected.
        /// </summary>
        protected virtual void NoMenuEntrySelected(object sender, EventArgs e)
        { }


        #endregion


        #region Drawing

        /// <summary>
        /// Draw this screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            Color color = new Color(255, 255, 255, TransitionAlpha);

            spriteBatch.Draw(background, origin, color);
            SelectedMenuEntry.Draw(this, gameTime);

            spriteBatch.End();
        }

        #endregion
    }
}

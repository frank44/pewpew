#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
#endregion

namespace Eve
{
    /// <summary>
    /// The pause screen temporarily freezes the game and gives options to 
    /// continue or exit.
    /// </summary>
    class PauseScreen : MenuScreen
    {
        #region Fields


        private Texture2D background, title;
        private Vector2 origin = new Vector2(0, 0);


        #endregion


        #region Menu Entries


        MenuEntry continueMenuEntry, exitMenuEntry;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public PauseScreen() : base()
        {
            IsPopup = true;

            continueMenuEntry = new MenuEntry();
            continueMenuEntry.Selected += ContinueMenuEntrySelected;
            MenuEntries.Add(continueMenuEntry);

            exitMenuEntry = new MenuEntry();
            exitMenuEntry.Selected += ExitMenuEntrySelected;
            MenuEntries.Add(exitMenuEntry);
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            // Load textures for the menu.
            background = content.Load<Texture2D>("Backgrounds/PauseScreen/Background");
            title = content.Load<Texture2D>("Backgrounds/PauseScreen/PauseTitle");
            continueMenuEntry.Texture = content.Load<Texture2D>("Sprites/PauseScreen/Continue");
            exitMenuEntry.Texture = content.Load<Texture2D>("Sprites/PauseScreen/Exit");

            base.LoadContent();
        }


        #endregion


        #region Updating


        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            if (Session.IsActive && (InputManager.IsActionTriggered(InputManager.Action.Pause) ||
                InputManager.IsActionTriggered(InputManager.Action.Back) ||
                InputManager.IsActionTriggered(InputManager.Action.ExitGame)))
            {
                Session.GameplayScreen.Freeze = false;
                ExitScreen();
            }
            base.HandleInput();
        }


        /// <summary>
        /// Event handler for when the Continue menu entry is selected.
        /// </summary>
        void ContinueMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                Session.GameplayScreen.Freeze = false;
                ExitScreen();
            }
        }

        
        /// <summary>
        /// Event handler for when the Exit menu entry is selected.
        /// </summary>
        void ExitMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                ScreenManager.AddScreen(new PauseConfirmationScreen());
            }
        }
                

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
            spriteBatch.Draw(title, origin, color);
            SelectedMenuEntry.Draw(this, gameTime);

            spriteBatch.End();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Platformer
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Fields


        //Animation of Eve on the main menu
        private AnimationPlayer sprite;
        private Animation runAnimation;
        private Vector2 spritePosition;

        //The rest of the background and foreground
        private Texture2D background, title;
        private Texture2D foreground;
        //private ScrollingBackground foreground;
        private Vector2 origin = new Vector2(0, 0);


        #endregion


        #region Menu Entries
        

        MenuEntry newGameMenuEntry, continueMenuEntry, extrasMenuEntry;


        #endregion


        #region Initialization
        

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen() : base() 
        {
            spritePosition = new Vector2(200, 375);

            newGameMenuEntry = new MenuEntry();
            newGameMenuEntry.Selected += NewGameMenuEntrySelected;
            MenuEntries.Add(newGameMenuEntry);

            continueMenuEntry = new MenuEntry();
            //continueMenuEntry.Selected += ContinueMenuEntrySelected;
            MenuEntries.Add(continueMenuEntry);

            extrasMenuEntry = new MenuEntry();
            //extrasMenuEntry.Selected += ExtraMenuEntrySelected;
            MenuEntries.Add(extrasMenuEntry);

            // start the menu music
            //AudioManager.PushMusic("MainTheme");
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            // Load textures for the menu.
            runAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Eve_running"), 0.15f, true);
            background = content.Load<Texture2D>("Backgrounds/Sky");
            title = content.Load<Texture2D>("Backgrounds/Title");
            foreground = content.Load<Texture2D>("Backgrounds/Cloud");
            newGameMenuEntry.Texture = content.Load<Texture2D>("Sprites/MainMenu/NewGame");
            continueMenuEntry.Texture = content.Load<Texture2D>("Sprites/MainMenu/Continue");
            extrasMenuEntry.Texture = content.Load<Texture2D>("Sprites/MainMenu/Extras");

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
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime,
                            bool otherScreenHasFocus,
                            bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            sprite.PlayAnimation(runAnimation);
        }

        
        /// <summary>
        /// Event handler for when the New Game menu entry is selected.
        /// </summary>
        void NewGameMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                ExitScreen();
            }

            ContentManager content = ScreenManager.Game.Content;

            //When loading a new game always start at level 0.
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen(0));
        }

        /*
        /// <summary>
        /// Event handler for when the Save Game menu entry is selected.
        /// </summary>
        void ExtraMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(
                new SaveLoadScreen(SaveLoadScreen.SaveLoadScreenMode.Save));
        }


        /// <summary>
        /// Event handler for when the Load Game menu entry is selected.
        /// </summary>
        void ContinueMenuEntrySelected(object sender, EventArgs e)
        {
            SaveLoadScreen loadGameScreen =
                new SaveLoadScreen(SaveLoadScreen.SaveLoadScreenMode.Load);
            loadGameScreen.LoadingSaveGame += new SaveLoadScreen.LoadingSaveGameHandler(
                loadGameScreen_LoadingSaveGame);
            ScreenManager.AddScreen(loadGameScreen);
        }


        /// <summary>
        /// Handle save-game-to-load-selected events from the SaveLoadScreen.
        /// </summary>
        void loadGameScreen_LoadingSaveGame(SaveGameDescription saveGameDescription)
        {
            if (Session.IsActive)
            {
                ExitScreen();
            }
            LoadingScreen.Load(ScreenManager, true,
                new GameplayScreen(saveGameDescription));
        }
        */

        #endregion

        #region Drawing

        /// <summary>
        /// Draw this screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            spriteBatch.Draw(background, origin, Color.White);
            spriteBatch.Draw(title, origin, Color.White);
            if (sprite.Animation != null)
            {
                sprite.Draw(gameTime, spriteBatch, spritePosition, SpriteEffects.None);
            }
            spriteBatch.Draw(foreground, origin, Color.White);
            SelectedMenuEntry.Draw(this, gameTime);

            spriteBatch.End();
        }
    
        #endregion
    }
}

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
#endregion

namespace Eve
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
            TransitionOnTime = TimeSpan.FromSeconds(1.0f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            spritePosition = new Vector2(200, 375);

            newGameMenuEntry = new MenuEntry();
            newGameMenuEntry.Selected += NewGameMenuEntrySelected;
            MenuEntries.Add(newGameMenuEntry);

            continueMenuEntry = new MenuEntry();
            continueMenuEntry.Selected += ContinueMenuEntrySelected;
            MenuEntries.Add(continueMenuEntry);

            extrasMenuEntry = new MenuEntry();
            //extrasMenuEntry.Selected += ExtraMenuEntrySelected;
            MenuEntries.Add(extrasMenuEntry);
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            // Load textures for the menu.
            runAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Eve_running"), 0.15f, true);
            background = content.Load<Texture2D>("Backgrounds/MainMenu/Sky");
            title = content.Load<Texture2D>("Backgrounds/MainMenu/Title");
            foreground = content.Load<Texture2D>("Backgrounds/MainMenu/Cloud");
            newGameMenuEntry.Texture = content.Load<Texture2D>("Sprites/MainMenu/NewGame");
            continueMenuEntry.Texture = content.Load<Texture2D>("Sprites/MainMenu/Continue");
            extrasMenuEntry.Texture = content.Load<Texture2D>("Sprites/MainMenu/Extras");

            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(content.Load<Song>("Sounds/MainMenu"));
            }
            catch { }
            
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
            
            //When loading a new game always start at level 0.
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
        }


        /// <summary>
        /// Event handler for when the Continue menu entry is selected.
        /// </summary>
        void ContinueMenuEntrySelected(object sender, EventArgs e)
        {
            SaveManager saveManager = new SaveManager(true);
            if (saveManager.StatisticsManager != null)
            {
                if (Session.IsActive)
                {
                    ExitScreen();
                }
                LoadingScreen.Load(ScreenManager, true, new GameplayScreen(saveManager));
            }
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

            Color color = new Color(255, 255, 255, TransitionAlpha);

            spriteBatch.Draw(background, origin, color);
            spriteBatch.Draw(title, origin, color);
            if (sprite.Animation != null)
            {
                sprite.Draw(gameTime, spriteBatch, spritePosition, color, SpriteEffects.None);
            }
            spriteBatch.Draw(foreground, origin, color);
            SelectedMenuEntry.Draw(this, gameTime);

            spriteBatch.End();
        }
    
        #endregion
    }
}

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
    /// The game over screen freezes the current game, displays a random 
    /// factoid concerning the level's theme, and gives the player the 
    /// option to continue or exit.
    /// </summary>
    class GameOverScreen : MenuScreen
    {
        #region Fields


        private Texture2D background, title, Eve;
        private Vector2 origin = new Vector2(0, 0);
        private Vector2 EvePosition;

        private Texture2D factAreaTexture;
        private Vector2 factAreaPosition;
        private string fact;
        private SpriteFont factFont;
        private Vector2 factPosition;


        #endregion


        #region Menu Entries


        MenuEntry tryAgainMenuEntry, exitMenuEntry;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public GameOverScreen(int type = -1) : base()
        {
            IsPopup = true;
            preventCancel = true;

            TransitionOnTime = TimeSpan.FromSeconds(1.5f);

            if (type == -1)
            {
                fact = FactoidManager.getRandomFact(Session.StatisticsManager.LevelIndex);
            }
            else
            {
                fact = FactoidManager.getRandomFact(Session.StatisticsManager.LevelIndex, type);
            }

            tryAgainMenuEntry = new MenuEntry();
            tryAgainMenuEntry.Selected += TryAgainMenuEntrySelected;
            MenuEntries.Add(tryAgainMenuEntry);

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
            background = content.Load<Texture2D>("Backgrounds/GameOver/Background");
            title = content.Load<Texture2D>("Backgrounds/GameOver/Title");
            Eve = content.Load<Texture2D>("Sprites/Player/Eve_dead");
            factAreaTexture = content.Load<Texture2D>("Backgrounds/GameOver/FactSheet");
            tryAgainMenuEntry.Texture = content.Load<Texture2D>("Sprites/GameOver/TryAgain");
            exitMenuEntry.Texture = content.Load<Texture2D>("Sprites/GameOver/Exit");

            EvePosition = new Vector2(115, 130);

            // Adjust the factoid to fit the texture and calculate the position of factoid
            factAreaPosition = new Vector2(320, 100);
            
            factFont = content.Load<SpriteFont>("Fonts/Fact");
            double height = factFont.MeasureString(fact).Y;
            double width = factAreaTexture.Width * 0.75;
            string [] words = fact.Split(' ');
            fact = "";
            foreach (string word in words)
            {
               if (fact.Length == 0)
               {
                   fact+=word;
               }
               else
               {
                   if (factFont.MeasureString(fact + " " + word).X > width)
                   {
                       fact += "\n" + word;
                   }
                   else
                   {
                       fact += " " + word;
                   }
               }
            }
            fact = "Did You Know?\n\n" + fact;
            Vector2 size = factFont.MeasureString(fact);
            factPosition = factAreaPosition + 
                           new Vector2((factAreaTexture.Width - size.X) / 2, 
                                       (factAreaTexture.Height - size.Y) / 2);


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
        /// Event handler for when the Continue menu entry is selected.
        /// </summary>
        void TryAgainMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                ExitScreen();
                Session.ReloadLevel();
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
            spriteBatch.Draw(Eve, EvePosition, color);
            spriteBatch.Draw(factAreaTexture, factAreaPosition, color);
            spriteBatch.DrawString(factFont, fact, factPosition, color);
            SelectedMenuEntry.Draw(this, gameTime);

            spriteBatch.End();
        }

        #endregion
    }
}

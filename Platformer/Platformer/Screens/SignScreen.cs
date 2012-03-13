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

namespace Platformer
{
    /// <summary>
    /// The sign screen pops up whenever the player reads a sign.
    /// </summary>
    class SignScreen : GameScreen
    {
        #region Fields


        private Texture2D background;
        private Vector2 origin = new Vector2(0, 0);

        private Texture2D factAreaTexture;
        private Vector2 factAreaPosition;
        private string fact;
        private SpriteFont factFont;
        private Vector2 factPosition;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor fills in the contents for a specific sign.
        /// </summary>
        public SignScreen(Sign sign)
            : base()
        {
            IsPopup = true;

            fact = FactoidManager.getFact(sign);
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            // Load textures for the menu.
            background = content.Load<Texture2D>("Backgrounds/GameOver/Background");
            factAreaTexture = content.Load<Texture2D>("Backgrounds/GameOver/FactSheet");
            
            // Adjust the factoid to fit the texture and calculate the position of factoid
            factAreaPosition = new Vector2(320, 100);

            factFont = content.Load<SpriteFont>("Fonts/Fact");
            double height = factFont.MeasureString(fact).Y;
            double width = factAreaTexture.Width * 0.75;
            string[] words = fact.Split(' ');
            fact = "";
            foreach (string word in words)
            {
                if (fact.Length == 0)
                {
                    fact += word;
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
            spriteBatch.Draw(factAreaTexture, factAreaPosition, color);
            spriteBatch.DrawString(factFont, fact, factPosition, color);
           
            spriteBatch.End();
        }

        #endregion
    }
}

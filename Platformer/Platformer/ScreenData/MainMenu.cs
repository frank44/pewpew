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
    class MainMenu : MenuScreen
    {
        #region Fields

        //Animation of Eve on the main menu
        private AnimationPlayer sprite;
        private Animation runAnimation;
        private Vector2 position;

        private Texture2D background, title, foreground;
        private Vector2 origin = new Vector2(0, 0);

        #endregion


        #region Initialization

        public MainMenu() : base() { }

        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            // Load textures for the menu.
            runAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Run"), 0.15f, true);
            background = content.Load<Texture2D>("Backgrounds/Sky");
            title = content.Load<Texture2D>("Backgrounds/Title");
            foreground = content.Load<Texture2D>("Backgrounds/Cloud");
            menu.Add(content.Load<Texture2D>("Sprites/MainMenu/NewGame"));
            menu.Add(content.Load<Texture2D>("Sprites/MainMenu/Continue"));
            menu.Add(content.Load<Texture2D>("Sprites/MainMenu/Extras"));

            position = new Vector2(275, 375);

            //Always start at New Game option
            menuIndex = 1;

            base.LoadContent();
        }

        #endregion


        #region Update and Draw

        public override void Update(GameTime gameTime,
                                    bool otherScreenHasFocus,
                                    bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (IsActive)
            {

            }
            sprite.PlayAnimation(runAnimation);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            spriteBatch.Draw(background, origin, Color.White);
            spriteBatch.Draw(title, origin, Color.White);
            sprite.Draw(gameTime, spriteBatch, position, SpriteEffects.None);
            spriteBatch.Draw(foreground, origin, Color.White);
            spriteBatch.Draw(menu[menuIndex], origin, Color.White);
        }
    
        #endregion
    }
}

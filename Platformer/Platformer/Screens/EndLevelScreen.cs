#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Platformer
{
    /// <summary>
    /// This screen pops up after the level is complete, displaying the current stats
    /// of the current level.
    /// </summary>
    class EndLevelScreen : GameScreen
    {
        #region Fields


        private Texture2D background;
        private Vector2 origin = new Vector2(0, 0);

        // The upperbound on the transition alpha for the background to fully load.
        private int backGroundTransitionOn = 55;

        private string outputString;
        private SpriteFont font;
        private string[] titleOfStats = 
            {
                "Number of Mistakes: ",
                "Number of Shots Fired: ",
                "Time Taken: "
            };
        private string[] outputOfStats;
        private Vector2[] textPositions;
        private bool[] setOff;
        private SoundEffect statDisplay;


        #endregion


        #region Initialization


        /// <summary>
        /// The end level screen is a popup and freezes the current session's game.
        /// </summary>
        public EndLevelScreen()
            : base()
        {
            TransitionOnTime = TimeSpan.FromSeconds(3.0f);
            IsPopup = true;
            Session.GameplayScreen.Freeze = true;

            outputOfStats = new string[titleOfStats.Length];
            textPositions = new Vector2[titleOfStats.Length];
            setOff = new bool[titleOfStats.Length];
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            // Load textures for the menu.
            background = content.Load<Texture2D>("Backgrounds/GameOver/Background");
            font = content.Load<SpriteFont>("Fonts/Fact");
            statDisplay = content.Load<SoundEffect>("Sounds/EndLevelScreen/StatAppears");

            // Adjust the output string to fit in the middle of the screen
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            outputOfStats[0] = string.Format("{0}", Session.StatisticsManager.DeathCount);
            outputOfStats[1] = string.Format("{0}", Session.StatisticsManager.ShotCount);
            outputOfStats[2] = Session.StatisticsManager.TotalTimeToString();
            
            outputString = "";
            for (int i = 0; i < titleOfStats.Length; i++)
            {
                outputString += titleOfStats[i] + outputOfStats[i] + '\n';
            }
            Vector2 size = font.MeasureString(outputString);
            Vector2 beginningPosition = (new Vector2(viewport.Width, viewport.Height) - size) / 2;

            for (int i = 0; i < titleOfStats.Length; i++)
            {
                Vector2 currentLineSize = font.MeasureString(titleOfStats[i] + outputOfStats[i]);
                textPositions[i] = beginningPosition + 
                    new Vector2((size.X - currentLineSize.X) / 2, i * size.Y / titleOfStats.Length);
            }

            base.LoadContent();
        }


        #endregion


        #region Updating


        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            if (InputManager.IsActionTriggered(InputManager.Action.Ok) ||
                InputManager.IsActionTriggered(InputManager.Action.Back))
            {
                Session.GameplayScreen.Freeze = false;
                ExitScreen();
                LoadingScreen.Load(ScreenManager, true, new GameplayScreen(Session.GameplayScreen.SaveManager));
            }
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

            // Draw the background.
            int backgroundAlpha = (int)Math.Min(255, TransitionAlpha * 255.0f / backGroundTransitionOn);
            //spriteBatch.Draw(background, origin, new Color(255, 255, 255, backgroundAlpha));

            // Draw the lines of stats.
            for (int i = 0; i < titleOfStats.Length; i++)
            {
                Console.WriteLine("{0} {1}", TransitionAlpha - backGroundTransitionOn, i * (TransitionAlpha - backGroundTransitionOn) / titleOfStats.Length);
                if (TransitionAlpha - backGroundTransitionOn > 0 &&
                    TransitionAlpha - backGroundTransitionOn > i * (TransitionAlpha - backGroundTransitionOn) / titleOfStats.Length)
                {
                    spriteBatch.DrawString(font, titleOfStats[i] + outputOfStats[i], textPositions[i], Color.White);
                    if (!setOff[i])
                    {
                        statDisplay.Play();
                        setOff[i] = true;
                    }
                }
            }
           
            spriteBatch.End();
        }

        #endregion
    }
}

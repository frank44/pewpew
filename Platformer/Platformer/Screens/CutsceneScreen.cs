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
    /// This screen is for displaying a cutscene.
    /// </summary>
    class CutsceneScreen : GameScreen
    {
        #region Fields


        /// <summary>
        /// The cutscene that is displayed.
        /// </summary>
        private VideoManager cutscene;

        
        /// <summary>
        /// The index of which cutscene is displayed.
        /// </summary>
        private int cutsceneIndex;


        private Vector2 origin = new Vector2(0, 0);


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor for the cutscene screen.
        /// </summary>
        public CutsceneScreen(int cutsceneIndex) : base()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5f);
            this.cutsceneIndex = cutsceneIndex;   
        }


        /// <summary>
        /// Load the video for this screen.
        /// </summary>
        public override void LoadContent()
        {
            string file = string.Format("Cutscenes/{0}", cutsceneIndex);
            cutscene = new VideoManager(ScreenManager.Game.Content, file, origin, new Vector2(1.0f, 1.0f), false);
            cutscene.Finished += CutsceneFinished;
            cutscene.Start();
            MediaPlayer.Stop();
            base.LoadContent();
        }


        #endregion


        #region Updating


        /// <summary>
        /// Update the video of the cutscene.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            cutscene.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        
        /// <summary>
        /// Cutscenes can be skipped.
        /// </summary>
        public override void HandleInput()
        {
            if ((InputManager.IsActionTriggered(InputManager.Action.Ok) ||
                InputManager.IsActionTriggered(InputManager.Action.Back)))
            {
                cutscene.Stop();
                cutscene.VideoCompleted();
            }
            base.HandleInput();
        }


        /// <summary>
        /// Event handler for when the cutscene has finished. These cutscenes always go to the next level.
        /// </summary>
        void CutsceneFinished(object sender, EventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, true, new GameplayScreen(new SaveManager(true)));
            LoadingScreen.Load(ScreenManager, true, new MainMenuScreen());
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
            cutscene.Draw(spriteBatch, gameTime, color);

            spriteBatch.End();
        }


        #endregion
    }
}
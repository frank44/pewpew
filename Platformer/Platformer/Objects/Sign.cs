#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
#endregion


namespace Platformer
{
    class Sign : Object
    {
        #region Properties


        /// <summary>
        /// Fact on the sign.
        /// </summary>
        private string factoid;


        /// <summary>
        /// Fact on the sign.
        /// </summary>
        public string Factoid
        {
            get { return factoid; }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new sign.
        /// </summary>
        public Sign(string fact, Vector2 position) : base("hospital_sign1", position)
        {
            factoid = fact;
        }


        /// <summary>
        /// Load the sprite of the sign.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();
            //Load the bounding rectangles of each frame here.
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Update the object.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            this.Update(gameTime);
        }


        /// <summary>
        /// Draws the object
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen, Color color)
        {
            base.Draw(gameTime, spriteBatch, screen, color);
        }


        #endregion
    }
}

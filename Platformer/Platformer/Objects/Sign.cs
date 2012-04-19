#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
#endregion

namespace Eve
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
            ObjectClass = ObjectClass.Activate;
        }


        /// <summary>
        /// Load the sprite of the sign.
        /// </summary>
        public override void LoadContent(bool isLooping = false)
        {
            base.LoadContent();
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Update the object.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        /// <summary>
        /// Draws the object
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen, Color color, bool freeze = true)
        {
            base.Draw(gameTime, spriteBatch, screen, color, freeze);
        }


        #endregion
    }
}

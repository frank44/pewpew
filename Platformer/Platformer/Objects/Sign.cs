#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
#endregion

namespace Eve
{
    /// <summary>
    /// Signs are objects that when activated create a checkpoint and display something
    /// for the player to read.
    /// </summary>
    class Sign : ActivatingObject
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
        public Sign(string objectType, Vector2 position, string fact, int objectID) : base(objectType, position, objectID)
        {
            factoid = fact;
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


        #region Methods


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Object Clone()
        {
            Sign clone = new Sign(objectType, Position, Factoid, objectID);
            clone.sprite = sprite;
            return clone;
        }


        /// <summary>
        /// Activates the object.
        /// </summary>
        public override void Activate()
        {
            Session.GameplayScreen.CheckpointReached(this);
        }


        #endregion
    }
}

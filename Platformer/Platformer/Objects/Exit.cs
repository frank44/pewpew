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
    /// An exit is a proximity trigger object that also functions as a transition
    /// to the next stage or level when the player touches the center.
    /// </summary>
    class Exit : ProximityTriggerObject
    {
        #region Properties


        /// <summary>
        /// When the player hits this point, an event for reaching the exit is called.
        /// </summary>
        private Point exitPoint;


        #endregion


        #region Event


        /// <summary>
        /// Event raised when the exit is reached by the player.
        /// </summary>
        public event EventHandler ExitReached;


        /// <summary>
        /// Method for raising the exit reached event.
        /// </summary>
        public void ReachedExit()
        {
            if (ExitReached != null)
                ExitReached(this, EventArgs.Empty);
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new object.
        /// </summary>
        public Exit(string objectType, Vector2 position, float radius, int objectID, bool front = false)
            : base(objectType, position, radius, objectID, front)
        {
            exitPoint = new Point((int)position.X, (int)position.Y);
        }


        #endregion


        #region Update


        /// <summary>
        /// Update the object.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // The player has reached the exit if they are standing on the ground and
            // his bounding rectangle contains the center of the exit tile. The animation
            // must be complete before advancing.
            if (Session.Level.Player.IsAlive && Session.Level.Player.IsOnGround && 
                Session.Level.Player.BoundingRectangle.Contains(exitPoint) && 
                sprite.FrameIndex == animation.FrameCount - 1)
            {
                ReachedExit();
            }
            base.Update(gameTime);
        }


        #endregion


        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Object Clone()
        {
            Exit clone = new Exit(objectType, Position, AreaOfTrigger.Radius, objectID, front);
            if (triggered == true)
            {
                clone.Trigger();
            }
            clone.sprite = sprite;
            clone.ExitReached = ExitReached;
            return clone;
        }


        #endregion
    }
}

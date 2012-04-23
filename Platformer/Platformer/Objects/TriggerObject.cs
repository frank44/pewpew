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
    /// These objects trigger an animation whenever their Trigger() function is called.
    /// </summary>
    class TriggerObject : Object
    {
        #region Properties


        /// <summary>
        /// Determines whether the object has been triggered.
        /// </summary>
        protected bool triggered;


        /// <summary>
        /// Determines whether the object can be reversed when triggered again.
        /// </summary>
        protected bool reversible;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new sign.
        /// </summary>
        public TriggerObject(string objectType, Vector2 position, int objectID, bool reversible = false)
            : base(objectType, position, objectID)
        {
            triggered = false;
            this.reversible = reversible;
            ObjectClass = ObjectClass.Trigger;
        }


        /// <summary>
        /// Load the sprite of the object.
        /// </summary>
        public override void LoadContent(bool isLooping = false)
        {
            base.LoadContent(isLooping);
        }
        

        #endregion


        #region Update and Draw


        /// <summary>
        /// Update the object.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // If the object's animation has finished and it is reversible, reverse the animation when triggered again.
            if (triggered == true && reversible == true
                && (sprite.FrameIndex == Animation.FrameCount-1 && sprite.direction == 1 
                || sprite.FrameIndex == 0 && sprite.direction == -1))
            {
                sprite.direction = -1 * sprite.direction;
                triggered = false;
            }
            base.Update(gameTime);
        }


        /// <summary>
        /// Triggers the object to do something.
        /// </summary>
        public virtual void Trigger()
        {
            triggered = true;
        }


        /// <summary>
        /// Draws the object
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen, Color color, bool freeze = true)
        {
            base.Draw(gameTime, spriteBatch, screen, color, !triggered);
        }


        #endregion


        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Object Clone()
        {
            TriggerObject clone = new TriggerObject(objectType, Position, objectID);
            if (triggered == true)
            {
                clone.Trigger();
            }
            clone.sprite = sprite;
            clone.reversible = reversible;
            return clone;
        }


        #endregion
    }
}

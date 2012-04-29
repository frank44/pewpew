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
    /// These objects trigger an animation whenever the player is within
    /// a certain distance from the object. They complete their animation 
    /// only once and stay on their last frame.
    /// </summary>
    class ProximityTriggerObject : TriggerObject
    {
        #region Properties


        /// <summary>
        /// If the player is within this area, the object triggers its animation.
        /// This area of trigger should be centered within the object.
        /// </summary>
        private Circle areaOfTrigger;


        /// <summary>
        /// If the player is within this area, the object triggers its animation.
        /// This area of trigger should be centered within the object.
        /// </summary>
        public Circle AreaOfTrigger
        {
            get { return areaOfTrigger; }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new object.
        /// </summary>
        public ProximityTriggerObject(string objectType, Vector2 position, float radius, int objectID, bool front = false)
            : base(objectType, position, objectID, front)
        {
            Vector2 centerOfObject = position;
            centerOfObject.Y -= sprite.Origin.Y/2;
            areaOfTrigger = new Circle(centerOfObject, radius);
            ObjectClass = ObjectClass.ProximityTrigger;
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
            base.Update(gameTime);
        }


        /// <summary>
        /// Triggers the object to do something.
        /// </summary>
        public override void Trigger()
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
            ProximityTriggerObject clone = new ProximityTriggerObject(objectType, Position, AreaOfTrigger.Radius, objectID, front);
            if (triggered == true)
            {
                clone.Trigger();
            }
            clone.sprite = sprite;
            return clone;
        }


        #endregion
    }
}

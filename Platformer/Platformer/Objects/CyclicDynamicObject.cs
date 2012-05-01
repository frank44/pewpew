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
    /// A dynamic object that restarts from its starting location whenever certain conditions are met.
    /// </summary>
    class CyclicDynamicObject : DynamicObject
    {
        #region Properties


        /// <summary>
        /// Determines whether the object restarts after colliding with something else.
        /// </summary>
        protected bool breakOnImpact;

        /// <summary>
        /// The starting position of the object.
        /// </summary>
        protected Vector2 startingPosition;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new object.
        /// </summary>
        public CyclicDynamicObject(string objectType, Vector2 startingPosition, Vector2 position, Vector2 displacement, int objectID, 
                                   bool breakOnImpact = false, bool reversible = false, bool isLooping = false)
            : base(objectType, position, displacement, objectID, reversible, isLooping)
        {
            this.breakOnImpact = breakOnImpact;
            if (breakOnImpact == true)
            {
                collision += RestartObject;
            }
            offLevel += RestartObject;
            this.startingPosition = startingPosition;
        }


        #endregion


        #region Update


        /// <summary>
        /// Called when the object needs to restart from its starting position.
        /// </summary>
        protected void RestartObject(object sender, EventArgs e)
        {
            position = startingPosition;
        }


        #endregion


        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Object Clone()
        {
            CyclicDynamicObject clone = new CyclicDynamicObject(objectType, startingPosition, Position, displacement, objectID, 
                                                                breakOnImpact, reversible, animation.IsLooping);
            clone.sprite = sprite;
            return clone;
        }


        #endregion
    }
}

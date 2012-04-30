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
    /// These objects are dynamic in the sense that they are affected by the physics of the game.
    /// </summary>
    class DynamicObject : Object
    {
        #region Properties


        /// <summary>
        /// Determines whether the object can be reversed under certain circumstances.
        /// </summary>
        protected bool reversible;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new object.
        /// </summary>
        public DynamicObject(string objectType, Vector2 position, int objectID, bool reversible = false)
            : base(objectType, position, objectID)
        {
            this.reversible = reversible;
            ObjectClass = ObjectClass.Dynamic;
        }


        #endregion


        #region Update


        /// <summary>
        /// Update the sprite and the characteristics of the current object. Apply physics here 
        /// on the object.
        /// </summary>
        public override void Update(GameTime gameTime) 
        { 
        
        }


        #endregion


        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Object Clone()
        {
            DynamicObject clone = new DynamicObject(objectType, Position, objectID, reversible);
            clone.sprite = sprite;
            return clone;
        }


        #endregion
    }
}

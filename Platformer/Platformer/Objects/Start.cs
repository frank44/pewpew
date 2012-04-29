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
    /// A start is a proximity trigger object that also functions as a beginning point
    /// for the player.
    /// </summary>
    class Start : ProximityTriggerObject
    {
        #region Properties


        /// <summary>
        /// The point where the player spawns in the stage.
        /// </summary>
        public Vector2 StartPoint
        {
            get { return Position; }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new object.
        /// </summary>
        public Start(string objectType, Vector2 position, float radius, int objectID, bool front = false)
            : base(objectType, position, radius, objectID, front)
        { }


        #endregion

        
        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Object Clone()
        {
            Start clone = new Start(objectType, Position, AreaOfTrigger.Radius, objectID, front);
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

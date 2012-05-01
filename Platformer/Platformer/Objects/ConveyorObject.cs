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
    /// An object that adds a displacement to other objects that are on top.
    /// </summary>
    class ConveyorObject : Object
    {
        #region Properties


        /// <summary>
        /// Determines whether the object can be reversed under certain circumstances.
        /// </summary>
        protected bool reversible;


        /// <summary>
        /// Determines the direction to displacement objects.
        /// </summary>
        protected int direction;


        /// <summary>
        /// How objects are displaced horizontally over time when on this object.
        /// </summary>
        protected float displacement;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new object.
        /// </summary>
        public ConveyorObject(string objectType, Vector2 position, float displacement, int objectID, bool reversible = false, bool isLooping = false)
            : base(objectType, position, objectID, false, isLooping)
        {
            this.displacement = displacement;
            this.reversible = reversible;
            ObjectClass = ObjectClass.Conveyor;
            // The direction is currently set to go right.
            direction = 1;
        }


        #endregion


        #region Update


        /// <summary>
        /// Update the sprite and the characteristics of the current object. Shift objects on top
        /// of this object in the appropriate direction.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Get the position of the top of this object.
            int top = (int)position.Y + animation.FrameHeight;
            foreach (Part part in Parts)
            {
                top = Math.Min(top, part.BoundingRectangle.Top);
            }

            foreach (Object currentObject in Session.Level.Objects)
            {
                // Do not check collisions with the same object.
                if (currentObject.ObjectID != ObjectID)
                {
                    foreach (Part part in currentObject.Parts)
                    {
                        // If the bottom of the object is touching the top of this object.
                        if (part.BoundingRectangle.Bottom == top)
                        {
                            currentObject.OffSet(displacement * direction, 0);
                            break;
                        }
                    }
                }
            }

            foreach (Enemy enemy in Session.Level.Enemies)
            {
                if (enemy.BoundingRectangle.Bottom == top)
                {
                    enemy.position.X += displacement * direction;
                }
            }

            if (Session.Level.Player.BoundingRectangle.Bottom == top)
            {
                Session.Level.Player.position.X += displacement * direction;
            }
        }


        #endregion


        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Object Clone()
        {
            ConveyorObject clone = new ConveyorObject(objectType, position, displacement, objectID, reversible, animation.IsLooping);
            clone.sprite = sprite;
            clone.direction = direction;
            return clone;
        }


        #endregion
    }
}

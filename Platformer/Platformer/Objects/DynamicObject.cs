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


        /// <summary>
        /// How the object is displaced over time.
        /// </summary>
        protected Vector2 displacement = new Vector2(-1, 5);


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
            LoadContent(true);
        }


        #endregion


        #region Update


        /// <summary>
        /// Update the sprite and the characteristics of the current object. Apply physics and collision detection
        /// here on the object.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            position += displacement;
            // Detecting collisions using the entire frame rather than the object's parts for now.
            
            Rectangle bounds = new Rectangle((int)(position.X-sprite.Origin.X), (int)(position.Y-sprite.Origin.Y), animation.FrameWidth, animation.FrameHeight);

            foreach (Object currentObject in Session.Level.Objects)
            {
                // Do not check collisions with the same object.
                if (currentObject.ObjectID != ObjectID)
                {
                    foreach (Part part in currentObject.Parts)
                    {
                        if (part.PartType != PartType.Passable)
                        {
                            Rectangle boundingRectangle = part.BoundingRectangle;
                            Vector2 intersection = RectangleExtensions.GetIntersectionDepth(bounds, boundingRectangle);

                            if (intersection != Vector2.Zero)
                            {
                                double depthX = intersection.X;
                                double depthY = intersection.Y;

                                if (Math.Abs(depthX) > Math.Abs(depthY))
                                {
                                    if (part.PartType == PartType.Solid)
                                    {
                                        position = new Vector2(position.X, position.Y + (float)depthY);
                                        bounds = new Rectangle((int)(position.X - sprite.Origin.X), (int)(position.Y - sprite.Origin.Y), animation.FrameWidth, animation.FrameHeight);
                                    }
                                }
                                else
                                {
                                    position = new Vector2(position.X + (float)depthX, position.Y);
                                    bounds = new Rectangle((int)(position.X - sprite.Origin.X), (int)(position.Y - sprite.Origin.Y), animation.FrameWidth, animation.FrameHeight);
                                }
                            }
                        }
                    }
                }
            }
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

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
        protected Vector2 displacement;


        #endregion


        #region Events


        /// <summary>
        /// An event for when the object collides with something.
        /// </summary>
        protected event EventHandler collision;


        /// <summary>
        /// A method to raise the event when a collision is detected.
        /// </summary>
        public void CollisionDetected()
        {
            if (collision != null)
            {
                collision(this, EventArgs.Empty);
            }
        }


        /// <summary>
        /// An event for when the object falls off the level.
        /// </summary>
        protected event EventHandler offLevel;


        /// <summary>
        /// A method to raise the event when the object falls off the level.
        /// </summary>
        public void OffLevelDetected()
        {
            if (offLevel != null)
            {
                offLevel(this, EventArgs.Empty);
            }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new object.
        /// </summary>
        public DynamicObject(string objectType, Vector2 position, Vector2 displacement, int objectID, bool reversible = false, bool isLooping = false)
            : base(objectType, position, objectID, false, isLooping)
        {
            this.displacement = displacement;
            this.reversible = reversible;
            ObjectClass = ObjectClass.Dynamic;
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

            // Find the tightest bounding box over the entire object.
            Vector2 topLeft = new Vector2(animation.FrameWidth, animation.FrameHeight);
            Vector2 bottomRight = new Vector2(0, 0);
            foreach (Part part in parts[sprite.FrameIndex])
            {
                topLeft.X = Math.Min(topLeft.X, part.BoundingRectangle.Left);
                topLeft.Y = Math.Min(topLeft.Y, part.BoundingRectangle.Top);
                bottomRight.X = Math.Max(bottomRight.X, part.BoundingRectangle.Right);
                bottomRight.Y = Math.Max(bottomRight.Y, part.BoundingRectangle.Bottom);
            }
            Rectangle boundingBox = new Rectangle((int)topLeft.X, (int)topLeft.Y,
                                                          (int)(bottomRight.X - topLeft.X),
                                                          (int)(bottomRight.Y - topLeft.Y));
            boundingBox.Offset((int)(position.X - sprite.Origin.X), (int)(position.Y - Sprite.Origin.Y));

            foreach (Object currentObject in Session.Level.Objects)
            {
                // Do not check collisions with the same object.
                if (currentObject.ObjectID != ObjectID)
                {
                    foreach (Part part in currentObject.Parts)
                    {
                        if (part.PartType == PartType.Solid)
                        {
                            Rectangle boundingRectangle = part.BoundingRectangle;
                            Vector2 intersection = RectangleExtensions.GetIntersectionDepth(boundingBox, boundingRectangle);

                            if (intersection != Vector2.Zero)
                            {
                                CollisionDetected();

                                double depthX = intersection.X;
                                double depthY = intersection.Y;

                                if (Math.Abs(depthX) > Math.Abs(depthY))
                                {
                                    position = new Vector2(position.X, position.Y + (float)depthY);
                                    boundingBox.Offset(0, (int)depthY);
                                }
                                else
                                {
                                    position = new Vector2(position.X + (float)depthX, position.Y);
                                    boundingBox.Offset((int)depthX, 0);
                                }
                            }
                        }
                    }
                }
            }

            // See if the object has fallen off the level.
            topLeft = new Vector2((int)(position.X - sprite.Origin.X), (int)(position.Y - Sprite.Origin.Y));
            if (topLeft.X + Animation.FrameWidth < 0 || topLeft.X > Session.Level.Width * 40
                || topLeft.Y + Animation.FrameHeight < 0 || topLeft.Y > Session.Level.Height * 32)
            {
                OffLevelDetected();
            }
        }


        #endregion


        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Object Clone()
        {
            DynamicObject clone = new DynamicObject(objectType, position, displacement, objectID, reversible, animation.IsLooping);
            clone.sprite = sprite;
            return clone;
        }


        #endregion
    }
}

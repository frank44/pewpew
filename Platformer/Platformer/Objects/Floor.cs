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
    /// These are the floors of the game. They are invisible since they don't require sprites.
    /// </summary>
    class Floor : Object
    {
        #region Properties


        /// <summary>
        /// The parts that encompass the floor.
        /// </summary>
        protected List<Part> floorParts;


        /// <summary>
        /// The parts that encompass the floor.
        /// </summary>
        public override List<Part> Parts
        {
            get { return floorParts; }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new object.
        /// </summary>
        public Floor(List<Part> parts, int objectID)
        {
            this.floorParts = new List<Part>();
            foreach (Part part in parts)
            {
                this.floorParts.Add(part.Clone());
            }
            this.objectID = objectID;
            ObjectClass = ObjectClass.Standing;
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Update the sprite and the characteristics of the current object.
        /// </summary>
        public override void Update(GameTime gameTime) { }


        /// <summary>
        /// Draws the object
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen, Color color, bool freeze = true) { }


        #endregion


        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Object Clone()
        {
            Floor clone = new Floor(Parts, objectID);
            return clone;
        }


        #endregion
    }
}

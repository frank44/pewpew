#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Eve
{
    /// <summary>
    /// Class to represent a solid part of an object.
    /// </summary>
    class PassablePart : Part
    {
        #region Initialization


        /// <summary>
        /// Constructor for creating a part.
        /// </summary>
        public PassablePart(Rectangle boundingRectangle)
            : base(boundingRectangle)
        {
            PartType = PartType.Passable;
        }


        /// <summary>
        /// Constructor for creating a part given a bounding box defined by an upper 
        /// left corner, a width, and a length.
        /// </summary>
        public PassablePart(int x, int y, int width, int height)
            : base(new Rectangle(x, y, width, height))
        {
            PartType = PartType.Passable;
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Creates a copy of the part.
        /// </summary>
        public override Part Clone()
        {
            Part clone = new PassablePart(BoundingRectangle);
            return clone;
        }


        #endregion
    }
}

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Platformer
{
    /// <summary>
    /// Class to represent a solid part of an object.
    /// </summary>
    class SolidPart : Part
    {
        #region Initialization


        /// <summary>
        /// Constructor for creating a part given a bounding box as a rectangle.
        /// </summary>
        public SolidPart(Rectangle boundingRectangle) 
            : base(boundingRectangle) 
        {
            PartType = PartType.Solid;
        }


        /// <summary>
        /// Constructor for creating a part given a bounding box defined by an upper 
        /// left corner, a width, and a length.
        /// </summary>
        public SolidPart(int x, int y, int width, int height)
            : base(new Rectangle(x, y, width, height))
        {
            PartType = PartType.Solid;
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Creates a copy of the part.
        /// </summary>
        public override Part Clone()
        {
            Part clone = new SolidPart(BoundingRectangle);
            return clone;
        }


        #endregion
    }
}

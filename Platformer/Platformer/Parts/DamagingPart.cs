#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Platformer
{
    /// <summary>
    /// Class to represent a damaging part of an object.
    /// </summary>
    class DamagingPart : Part
    {
        #region Initialization


        /// <summary>
        /// Constructor for creating a part.
        /// </summary>
        public DamagingPart(Rectangle boundingRectangle)
            : base(boundingRectangle)
        {
            PartType = PartType.Damaging;
        }


        /// <summary>
        /// Constructor for creating a part given a bounding box defined by an upper 
        /// left corner, a width, and a length.
        /// </summary>
        public DamagingPart(int x, int y, int width, int height)
            : base(new Rectangle(x, y, width, height))
        {
            PartType = PartType.Damaging;
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Creates a copy of the part.
        /// </summary>
        public override Part Clone()
        {
            Part clone = new DamagingPart(BoundingRectangle);
            return clone;
        }


        #endregion
    }
}

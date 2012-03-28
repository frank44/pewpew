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
    class PlatformPart : Part
    {
        #region Initialization


        /// <summary>
        /// Constructor for creating a part.
        /// </summary>
        public PlatformPart(Rectangle boundingRectangle)
            : base(boundingRectangle)
        {
            PartType = PartType.Platform;
        }


        /// <summary>
        /// Constructor for creating a part given a bounding box defined by an upper 
        /// left corner, a width, and a length.
        /// </summary>
        public PlatformPart(int x, int y, int width, int height)
            : base(new Rectangle(x, y, width, height))
        {
            PartType = PartType.Platform;
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Creates a copy of the part.
        /// </summary>
        public override Part Clone()
        {
            Part clone = new PlatformPart(BoundingRectangle);
            return clone;
        }


        #endregion
    }
}

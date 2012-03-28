#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Platformer
{
    /// <summary>
    /// Enum describes the part.
    /// </summary>
    public enum PartType
    {
        Bouncy,
        Damaging,
        Passable,
        Platform,
        Solid,
    }

    /// <summary>
    /// Class to represent a part of an object.
    /// </summary>
    public abstract class Part
    {
        #region Properties


        /// <summary>
        /// The bounding rectangle of a part.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get { return boundingRectangle; }
            set { boundingRectangle = value; }
        }

        private Rectangle boundingRectangle;


        /// <summary>
        /// Gets the current type of the part.
        /// </summary>
        public PartType PartType
        {
            get { return partType; }
            protected set { partType = value; }
        }

        private PartType partType;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor for creating a part.
        /// </summary>
        public Part(Rectangle boundingRectangle)
        {
            this.boundingRectangle = boundingRectangle;
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Creates a copy of the part.
        /// </summary>
        public virtual Part Clone() 
        {
            return null;
        }


        #endregion
    }
}

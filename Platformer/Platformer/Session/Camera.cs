#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Eve
{
    /// <summary>
    /// The camera focuses on a 2D space in the game. Anything within the camera is displayed
    /// on the screen.
    /// </summary>
    class Camera
    {
        #region Properties


        /// <summary>
        /// The position of the camera.
        /// </summary>
        private Vector2 position;


        /// <summary>
        /// The position of the camera.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }

        #endregion


        #region Initialization


        public Camera()
        {
            position = new Vector2();
        }


        #endregion


        #region Update


        /// <summary>
        ///  Updates the camera position based on the given point. The camera adjusts so that the given
        ///  point is centered and so that it does not go past the given boundaries.
        ///  </summary>
        public void Update(GameTime gameTime, Vector2 point, Vector2 boundaries, Viewport window)
        {
            //Screen would be too far to the left, so fix the screen.
            if (point.X - window.Width / 2.0f <= 0)
                position.X = 0;
            //Screen would be too far to the right.
            else if (point.X + window.Width / 2.0f >= boundaries.X)
                position.X = boundaries.X - window.Width;
            //Otherwise set the screen so that the point is horizontally in the middle.
            else
                position.X = point.X - window.Width / 2.0f;

            //Screen would be too far down, so fix the screen.
            if (point.Y + window.Height / 2.0f >= boundaries.Y)
                position.Y = boundaries.Y - window.Height;
            //Screen would be too far up.
            else if (point.Y - window.Height / 2.0f <= 0)
                position.Y = 0;
            //Otherwise set the screen so that the point is vertically in the middle.
            else
                position.Y = point.Y - window.Height / 2.0f;
        }


        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer
{
    class Sign : Object
    {
        #region Constants


        /// <summary>
        /// The level that the sign is located in.
        /// </summary>
        private int levelIndex;


        /// <summary>
        /// The level that the sign is located in.
        /// </summary>
        public int LevelIndex
        {
            get { return levelIndex; }
        }


        /// <summary>
        /// The type of factoid for the sign.
        /// </summary>
        private int type;


        /// <summary>
        /// The type of factoid for the sign.
        /// </summary>
        public int Type
        {
            get { return type; }
        }


        /// <summary>
        /// The index of the factoid for the sign.
        /// </summary>
        private int index;


        /// <summary>
        /// The index of the factoid for the sign.
        /// </summary>
        public int Index
        {
            get { return index; }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new sign.
        /// </summary>
        public Sign(int levelIndex, int type, int index, Vector2 position) : base("w.e", position)
        {
            this.levelIndex = levelIndex;
            this.type = type;
            this.index = index;
            Damaging = false;
            Passable = true;
            Bouncy = false;
        }


        /// <summary>
        /// Load the sprite of the sign.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();
            
            //Load the bounding rectangles of each frame here.
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Update the object.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            this.Update(gameTime);
        }


        /// <summary>
        /// Draws the object
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen)
        {
            this.Draw(gameTime, spriteBatch, screen);
        }


        #endregion
    }
}

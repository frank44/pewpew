using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


        public Sign(int levelIndex, int type, int index) : base(
        {

            this.levelIndex = levelIndex;
            this.type = type;
            this.index = index;
        }


        #endregion
    }
}

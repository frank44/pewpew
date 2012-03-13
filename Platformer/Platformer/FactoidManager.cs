#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;
#endregion

namespace Platformer
{
    /// <summary>
    /// This class handles all the factoids used by the game.
    /// </summary>
    public static class FactoidManager
    {
        #region Constants


        private static List <string> [,] factoids;


        #endregion


        #region Initialization


        /// <summary>
        /// Creates all the facts in the appropriate list.
        /// </summary>
        public static void Initialize()
        {
            int totalLevels = PlatformerGame.totalLevels;

            factoids = new List<string>[totalLevels, 3];
            for (int levelIndex = 0; levelIndex < totalLevels; levelIndex++)
            {
                string levelPath = string.Format("Content/Factoids/{0}.txt", levelIndex);
                StreamReader reader = new StreamReader(TitleContainer.OpenStream(levelPath));
                int type = -1;
                string line = reader.ReadLine();
                while (line != null)
                {
                    //If the line does not begin with *, then it is the beginning of a new type of facts
                    if (line[0] != '*')
                    {
                        type++;
                        factoids[levelIndex, type] = new List<string>();
                    }
                    else
                    {
                        factoids[levelIndex, type].Add(line.Substring(1).Trim());
                    }
                    line = reader.ReadLine();
                }
            }
        }


        #endregion


        #region Methods


        /// <summary>
        /// Gets a random factoid from the current level.
        /// </summary>
        public static string getRandomFact(int levelIndex)
        {
            int type = Session.Random.Next(3);
            return getRandomFact(levelIndex, type);
        }


        /// <summary>
        /// Gets a random factoid from the current level of a specific type.
        /// </summary>
        public static string getRandomFact(int levelIndex, int type)
        {
            int index = Session.Random.Next(factoids[levelIndex, type].Count);
            return getFact(levelIndex, type, index);
        }


        /// <summary>
        /// Gets a specific factoid from a sign.
        /// </summary>
        public static string getFact(Sign sign)
        {
            return getFact(sign.LevelIndex, sign.Type, sign.Index);
        }


        /// <summary>
        /// Gets a specific factoid.
        /// </summary>
        public static string getFact(int levelIndex, int type, int index)
        {
            return factoids[levelIndex, type][index];
        }


        #endregion
    }
}

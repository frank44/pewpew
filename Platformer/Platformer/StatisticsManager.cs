#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace Platformer
{
    /// <summary>
    /// This class handles all the stats and saving features.
    /// </summary>
    class StatisticsManager
    {
        #region Level Statistics


        /// <summary>
        /// Current level the player is in.
        /// </summary>
        private int levelIndex;


        /// <summary>
        /// Current level the player is in.
        /// </summary>
        public int LevelIndex
        {
            get { return levelIndex; }
            set { levelIndex = value; }
        }


        /// <summary>
        /// Current position the player is in.
        /// </summary>
        private Vector2 position;


        /// <summary>
        /// Current position the player is in.
        /// </summary>
        public Vector2 Position
        {
            get{ return position; }
            set{ position = value; }
        }


        #endregion
        
        
        #region Death Statistics


        /// <summary>
        /// Number of times player has died.
        /// </summary>
        private int deathCount;


        /// <summary>
        /// Number of times player has died.
        /// </summary>
        public int DeathCount
        {
            get { return deathCount; }
            set { deathCount = value; }
        }


        /// <summary>
        /// Increase the number of times a player has died by 1.
        /// </summary>
        public void IncreaseDeathCount()
        {
            deathCount++;
        }


        /// <summary>
        /// Reset the number of times a player has died to 0.
        /// </summary>
        public void ResetDeathCount()
        {
            deathCount = 0;
        }


        #endregion


        #region Time Statistics


        /// <summary>
        /// Total elapsed time in seconds since the beginning of the game.
        /// </summary>
        private int totalTime;


        /// <summary>
        /// Total elapsed time in seconds since the beginning of the game.
        /// </summary>
        public int TotalTime
        {
            get { return totalTime; }
            set { totalTime = value; }
        }


        /// <summary>
        /// Increase the total time by the specified amount of seconds.
        /// </summary>
        public void IncreaseTotalTime(int addSeconds)
        {
            totalTime += addSeconds;
        }


        /// <summary>
        /// Reset the total time to 0 seconds.
        /// </summary>
        public void ResetTotalTime()
        {
            totalTime = 0;
        }



        #endregion


        #region Initialization


        /// <summary>
        /// Constructor that resets all stats if there is no specified statistics manager 
        /// and loads all data if there is one.
        /// </summary>
        public StatisticsManager(StatisticsManager statisticsManager = null)
        {
            if(statisticsManager == null)
            {
                ResetDeathCount();
                ResetTotalTime();
                levelIndex = 0;
            }
            else
            {
                copyStats(statisticsManager);
            }
        }


        /// <summary>
        /// Copy stats from a provided statistics manager.
        /// </summary>
        private void copyStats(StatisticsManager statisticsManager)
        {
            levelIndex = statisticsManager.levelIndex;
            position = statisticsManager.position;
            deathCount = statisticsManager.deathCount;
            totalTime = statisticsManager.totalTime;
        }

        #endregion
    }
}

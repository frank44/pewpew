﻿#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace Platformer
{
    /// <summary>
    /// This class handles all the stats.
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
        }


        /// <summary>
        /// Sets the current level index to the specified value.
        /// </summary>
        public void SetLevelIndex(int levelIndex)
        {
            this.levelIndex = levelIndex;
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
        }


        /// <summary>
        /// Sets the current position the player is in.
        /// </summary>
        public void SetPosition(Vector2 position)
        {
            this.position = position;
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


        /// <summary>
        /// Sets the death count to the specified amount.
        /// </summary>
        public void SetDeathCount(int deathCount)
        {
            this.deathCount = deathCount;
        }


        #endregion


        #region Time Statistics


        /// <summary>
        /// Total elapsed time in seconds since the beginning of the game.
        /// </summary>
        private double totalTime;


        /// <summary>
        /// Total elapsed time in seconds since the beginning of the game.
        /// </summary>
        public double TotalTime
        {
            get { return totalTime; }
        }


        /// <summary>
        /// Increase the total time by the specified amount of seconds.
        /// </summary>
        public void IncreaseTotalTime(double addSeconds)
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


        /// <summary>
        /// Sets the total time to the specified amount.
        /// </summary>
        public void SetTotalTime(double totalTime)
        {
            this.totalTime = totalTime;
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
                // Set position to be an impossible value. This means that the player
                // needs to be positioned based on the level's starting position.
                position = new Vector2(-1, -1);
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

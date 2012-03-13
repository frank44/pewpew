#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework;
#endregion

namespace Platformer
{
    /// <summary>
    /// This class deals with the saving and loading of files.
    /// </summary>
    class SaveManager
    {
        #region Constants


        // For now we only have one file for the game.
        private const string fileName = "SaveFile.txt";
        private const string levelPath = "Content/";


        /// <summary>
        /// The current stats of the latest save.
        /// </summary>
        private StatisticsManager statisticsManager;


        /// <summary>
        /// The current stats of the latest save.
        /// </summary>
        public StatisticsManager StatisticsManager
        {
            get { return statisticsManager; }
        }
        

        #endregion


        #region Initialization


        /// <summary>
        /// Construct the manager with the basic save file.
        /// </summary>
        public SaveManager(bool loadData = false)
        {
            if (!loadData || IsEmpty())
            {
                statisticsManager = null;
            }
            else
            {
                LoadData();
            }
        }


        #endregion


        #region Methods


        /// <summary>
        /// Check to see if save file is empty.
        /// </summary>
        public bool IsEmpty()
        {
            StreamReader reader = new StreamReader(levelPath + fileName);
            string line = reader.ReadLine();
            reader.Close();
            return line == null;
        }


        /// <summary>
        /// Load the data from the save file.
        /// </summary>
        public void LoadData()
        {
            StreamReader reader = new StreamReader(levelPath+fileName);
            string line = reader.ReadLine();
            statisticsManager = new StatisticsManager();

            while (line != null)
            {
                if (line == "Level Index:")
                {
                    StatisticsManager.SetLevelIndex(int.Parse(reader.ReadLine()));
                }
                else if (line == "Position:")
                {
                    string[] position = reader.ReadLine().Split(' ');
                    StatisticsManager.SetPosition(new Vector2(float.Parse(position[0]), float.Parse(position[1])));
                }
                else if (line == "Death Count:")
                {
                    StatisticsManager.SetDeathCount(int.Parse(reader.ReadLine()));
                }
                else if (line == "Total Time:")
                {
                    StatisticsManager.SetTotalTime(float.Parse(reader.ReadLine()));
                }
                line = reader.ReadLine();
            }
        }


        /// <summary>
        /// Override the old save data with the current save.
        /// </summary>
        public void SaveData()
        {
            StreamWriter writer = new StreamWriter(levelPath + fileName);
            writer.WriteLine("Level Index:");
            writer.WriteLine(statisticsManager.LevelIndex);
            writer.WriteLine("Position:");
            writer.WriteLine(string.Format("{0} {1}", statisticsManager.Position.X, statisticsManager.Position.Y));
            writer.WriteLine("Death Count:");
            writer.WriteLine(statisticsManager.DeathCount);
            writer.WriteLine("Total Time:");
            writer.WriteLine(statisticsManager.TotalTime);
            writer.Close();
        }


        /// <summary>
        /// Set the stats of the current save.
        /// </summary>
        public void SetStatistics(StatisticsManager statisticsManager)
        {
            this.statisticsManager = new StatisticsManager(statisticsManager);
        }

        #endregion
    }
}

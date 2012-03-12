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
    class SaveManager
    {
        #region Constants


        private string fileName;
        private string levelPath;


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
            set { statisticsManager = value; }
        }
        

        #endregion


        #region Initialization


        /// <summary>
        /// Construct the manager with the basic save file.
        /// </summary>
        public SaveManager()
        {
            fileName = "SaveFile.txt";
            levelPath = "Content/" + fileName;
            if (IsEmpty())
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
            StreamReader reader = new StreamReader(TitleContainer.OpenStream(levelPath));
            string line = reader.ReadLine();
            reader.Close();
            return line == null;
        }


        /// <summary>
        /// Load the data from the save file.
        /// </summary>
        public void LoadData()
        {
            StreamReader reader = new StreamReader(TitleContainer.OpenStream(levelPath));
            string line = reader.ReadLine();
            StatisticsManager = new StatisticsManager();

            while (line != null)
            {
                if (line == "Level Index:")
                {
                    StatisticsManager.LevelIndex = int.Parse(reader.ReadLine());
                }
                else if (line == "Position:")
                {
                    string[] position = reader.ReadLine().Split(' ');
                    StatisticsManager.Position = new Vector2(float.Parse(position[0]), float.Parse(position[1]));
                }
                else if (line == "Death Count:")
                {
                    StatisticsManager.DeathCount = int.Parse(reader.ReadLine());
                }
                else if (line == "Total Time:")
                {
                    StatisticsManager.TotalTime = int.Parse(reader.ReadLine());
                }
                line = reader.ReadLine();
            }
        }


        #endregion
    }
}

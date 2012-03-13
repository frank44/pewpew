#region File Description
//-----------------------------------------------------------------------------
// Session.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Platformer
{
    class Session
    {
        #region Singleton


        /// <summary>
        /// The single Session instance that can be active at a time.
        /// </summary>
        private static Session singleton;


        #endregion


        #region Level


        /// <summary>
        /// The level that the current session is in.
        /// </summary>
        private Level level;


        /// <summary>
        /// Reload the level from the last checkpoint.
        /// </summary>
        public static void ReloadLevel()
        {
            Level.StartNewLife();
        }


        /// <summary>
        /// The level that the current session is in.
        /// </summary>
        public static Level Level
        {
            get { return (singleton == null ? null : singleton.level); }
            set { singleton.level = value; }
        }

        
        /// <summary>
        /// Load the level at based on the information from the stats in the current session.
        /// </summary>
        public static void LoadLevel()
        {
            Level = new Level(ScreenManager.Game.Content, ScreenManager.GraphicsDevice.Viewport);
        }


        #endregion


        #region User Interface Data


        /// <summary>
        /// The ScreenManager used to manage all UI in the game.
        /// </summary>
        private ScreenManager screenManager;

        /// <summary>
        /// The ScreenManager used to manage all UI in the game.
        /// </summary>
        public static ScreenManager ScreenManager
        {
            get { return (singleton == null ? null : singleton.screenManager); }
        }


        /// <summary>
        /// The GameplayScreen object that created this session.
        /// </summary>
        private GameplayScreen gameplayScreen;


        /// <summary>
        /// The GameplayScreen object that created this session.
        /// </summary>
        public static GameplayScreen GameplayScreen
        {
            get { return (singleton == null ? null : singleton.gameplayScreen); }
        }


        /// <summary>
        /// The current stats of the current session.
        /// </summary>
        private StatisticsManager statisticsManager;


        /// <summary>
        /// The current stats of the current session.
        /// </summary>
        public static StatisticsManager StatisticsManager
        {
            get { return singleton.statisticsManager; }
        }


        // HUD information
        private SpriteFont hudFont;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);


        #endregion


        #region State Data


        /// <summary>
        /// Returns true if there is an active session.
        /// </summary>
        public static bool IsActive
        {
            get { return singleton != null; }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Private constructor of a Session object.
        /// </summary>
        private Session(ScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameter
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (gameplayScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // assign the parameter
            this.screenManager = screenManager;
            this.gameplayScreen = gameplayScreen;

            //HUD information
            ContentManager Content = screenManager.Game.Content;
            hudFont = Content.Load<SpriteFont>("Fonts/Hud");
        }


        #endregion


        #region Updating


        /// <summary>
        /// Update the session for this frame.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            // check the singleton
            if (singleton == null)
            {
                return;
            }
            StatisticsManager.IncreaseTotalTime(gameTime.ElapsedGameTime.TotalSeconds);

            Level.Update(gameTime);
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draws the session environment to the screen
        /// </summary>
        public static void Draw(GameTime gameTime, Color color, bool freeze = false)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            Level.Draw(gameTime, spriteBatch, color, freeze);
            spriteBatch.End();

            singleton.DrawHud();
        }

        
        private void DrawHud()
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle titleSafeArea = viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            string timeString = "TIME: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
            Color timeColor;
            if (level.TimeRemaining > WarningTime ||
                level.ReachedExit ||
                (int)level.TimeRemaining.TotalSeconds % 2 == 0)
            {
                timeColor = Color.Yellow;
            }
            else
            {
                timeColor = Color.Red;
            }
            DrawShadowedString(hudFont, timeString, hudLocation, timeColor);

            // Draw score
            float timeHeight = hudFont.MeasureString(timeString).Y;
            DrawShadowedString(hudFont, "SCORE: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.Yellow);

            // Determine the status overlay message to show.
            Texture2D status = null;


            if (status != null)
            {
                // Draw status message.
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                spriteBatch.Begin();
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
                spriteBatch.End();
            }
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
            spriteBatch.End();
        }


        #endregion


        #region Starting a Session


        /// <summary>
        /// Start a new session based on the data provided.
        /// </summary>
        public static void StartSession(StatisticsManager statisticsManager, 
            ScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameters
            if (statisticsManager == null)
            {
                throw new ArgumentNullException("statisticsManager");
            }
            if (statisticsManager.LevelIndex < 0 || statisticsManager.LevelIndex >= PlatformerGame.totalLevels)
            {
                throw new ArgumentNullException("levelIndex");
            }
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (gameplayScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // end any existing session
            EndSession();

            // create a new singleton
            singleton = new Session(screenManager, gameplayScreen);

            // load the singleton's stats with the provided stats
            singleton.statisticsManager = statisticsManager;

            // set up the initial level
            LoadLevel();
        }


        #endregion


        #region Ending a Session


        /// <summary>
        /// End the current session.
        /// </summary>
        public static void EndSession()
        {
            // exit the gameplay screen
            // -- store the gameplay session, for re-entrance
            if (singleton != null)
            {
                GameplayScreen gameplayScreen = singleton.gameplayScreen;
                singleton.gameplayScreen = null;

                // clear the singleton
                singleton = null;

                if (gameplayScreen != null)
                {
                    gameplayScreen.ExitScreen();
                }
            }
        }


        #endregion

        
        #region Saving the Session
        /*

        /// <summary>
        /// Save the current state of the session.
        /// </summary>
        /// <param name="overwriteDescription">
        /// The description of the save game to over-write, if any.
        /// </param>
        public static void SaveSession(SaveGameDescription overwriteDescription)
        {
            // retrieve the storage device, asynchronously
            GetStorageDevice(delegate(StorageDevice storageDevice)
            {
                SaveSessionResult(storageDevice, overwriteDescription);
            });
        }


        /// <summary>
        /// Save the current state of the session, with the given storage device.
        /// </summary>
        /// <param name="storageDevice">The chosen storage device.</param>
        /// <param name="overwriteDescription">
        /// The description of the save game to over-write, if any.
        /// </param>
        private static void SaveSessionResult(StorageDevice storageDevice,
            SaveGameDescription overwriteDescription)
        {
            // check the parameter
            if ((storageDevice == null) || !storageDevice.IsConnected)
            {
                return;
            }

            // open the container
            using (StorageContainer storageContainer =
                OpenContainer(storageDevice))
            {
                string filename;
                string descriptionFilename;
                // get the filenames
                if (overwriteDescription == null)
                {
                    int saveGameIndex = 0;
                    string testFilename;
                    do
                    {
                        saveGameIndex++;
                        testFilename = "SaveGame" + saveGameIndex.ToString() + ".xml";
                    }
                    while (storageContainer.FileExists(testFilename));
                    filename = testFilename;
                    descriptionFilename = "SaveGameDescription" +
                        saveGameIndex.ToString() + ".xml";
                }
                else
                {
                    filename = overwriteDescription.FileName;
                    descriptionFilename = "SaveGameDescription" +
                        Path.GetFileNameWithoutExtension(
                        overwriteDescription.FileName).Substring(8) + ".xml";
                }
                using (Stream stream = storageContainer.OpenFile(filename, FileMode.Create))
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stream))
                    {
                        // <rolePlayingGameData>
                        xmlWriter.WriteStartElement("rolePlayingGameSaveData");

                        // write the map information
                        xmlWriter.WriteStartElement("mapData");
                        xmlWriter.WriteElementString("mapContentName",
                            TileEngine.Map.AssetName);
                        new XmlSerializer(typeof(PlayerPosition)).Serialize(
                            xmlWriter, TileEngine.PartyLeaderPosition);
                        new XmlSerializer(typeof(List<WorldEntry<Chest>>)).Serialize(
                            xmlWriter, singleton.removedMapChests);
                        new XmlSerializer(
                            typeof(List<WorldEntry<FixedCombat>>)).Serialize(
                            xmlWriter, singleton.removedMapFixedCombats);
                        new XmlSerializer(typeof(List<WorldEntry<Player>>)).Serialize(
                            xmlWriter, singleton.removedMapPlayerNpcs);
                        new XmlSerializer(typeof(List<ModifiedChestEntry>)).Serialize(
                            xmlWriter, singleton.modifiedMapChests);
                        xmlWriter.WriteEndElement();

                        // write the quest information
                        xmlWriter.WriteStartElement("questData");
                        xmlWriter.WriteElementString("questLineContentName",
                            singleton.questLine.AssetName);
                        xmlWriter.WriteElementString("currentQuestIndex",
                            singleton.currentQuestIndex.ToString());
                        new XmlSerializer(typeof(List<WorldEntry<Chest>>)).Serialize(
                            xmlWriter, singleton.removedQuestChests);
                        new XmlSerializer(
                            typeof(List<WorldEntry<FixedCombat>>)).Serialize(
                            xmlWriter, singleton.removedQuestFixedCombats);
                        new XmlSerializer(typeof(List<ModifiedChestEntry>)).Serialize(
                            xmlWriter, singleton.modifiedQuestChests);
                        xmlWriter.WriteElementString("currentQuestStage",
                            IsQuestLineComplete ?
                            Quest.QuestStage.NotStarted.ToString() :
                            singleton.quest.Stage.ToString());
                        xmlWriter.WriteEndElement();

                        // write the party data
                        new XmlSerializer(typeof(PartySaveData)).Serialize(xmlWriter,
                            new PartySaveData(singleton.party));

                        // </rolePlayingGameSaveData>
                        xmlWriter.WriteEndElement();
                    }
                }

                // create the save game description
                SaveGameDescription description = new SaveGameDescription();
                description.FileName = Path.GetFileName(filename);
                description.ChapterName = IsQuestLineComplete ? "Quest Line Complete" :
                    Quest.Name;
                description.Description = DateTime.Now.ToString();
                using (Stream stream =
                    storageContainer.OpenFile(descriptionFilename, FileMode.Create))
                {
                    new XmlSerializer(typeof(SaveGameDescription)).Serialize(stream,
                        description);
                }
            }
        }

        */
        #endregion
        

        #region Save Game Descriptions
        /*

        /// <summary>
        /// Save game descriptions for the current set of save games.
        /// </summary>
        private static List<SaveGameDescription> saveGameDescriptions = null;

        /// <summary>
        /// Save game descriptions for the current set of save games.
        /// </summary>
        public static List<SaveGameDescription> SaveGameDescriptions
        {
            get { return saveGameDescriptions; }
        }


        /// <summary>
        /// The maximum number of save-game descriptions that the list may hold.
        /// </summary>
        public const int MaximumSaveGameDescriptions = 5;


        /// <summary>
        /// XML serializer for SaveGameDescription objects.
        /// </summary>
        private static XmlSerializer saveGameDescriptionSerializer =
            new XmlSerializer(typeof(SaveGameDescription));


        /// <summary>
        /// Refresh the list of save-game descriptions.
        /// </summary>
        public static void RefreshSaveGameDescriptions()
        {
            // clear the list
            saveGameDescriptions = null;

            // retrieve the storage device, asynchronously
            GetStorageDevice(RefreshSaveGameDescriptionsResult);
        }


        /// <summary>
        /// Asynchronous storage-device callback for 
        /// refreshing the save-game descriptions.
        /// </summary>
        private static void RefreshSaveGameDescriptionsResult(
            StorageDevice storageDevice)
        {
            // check the parameter
            if ((storageDevice == null) || !storageDevice.IsConnected)
            {
                return;
            }

            // open the container
            using (StorageContainer storageContainer =
                OpenContainer(storageDevice))
            {
                saveGameDescriptions = new List<SaveGameDescription>();
                // get the description list
                string[] filenames =
                    storageContainer.GetFileNames("SaveGameDescription*.xml");
                // add each entry to the list
                foreach (string filename in filenames)
                {
                    SaveGameDescription saveGameDescription;

                    // check the size of the list
                    if (saveGameDescriptions.Count >= MaximumSaveGameDescriptions)
                    {
                        break;
                    }
                    // open the file stream
                    using (Stream fileStream = storageContainer.OpenFile(filename, FileMode.Open))
                    {
                        // deserialize the object
                        saveGameDescription =
                            saveGameDescriptionSerializer.Deserialize(fileStream)
                            as SaveGameDescription;
                        // if it's valid, add it to the list
                        if (saveGameDescription != null)
                        {
                            saveGameDescriptions.Add(saveGameDescription);
                        }
                    }
                }
            }
        }

        */
        #endregion


        #region Random


        /// <summary>
        /// The random-number generator used with game events.
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// The random-number generator used with game events.
        /// </summary>
        public static Random Random
        {
            get { return random; }
        }


        #endregion
    }
}

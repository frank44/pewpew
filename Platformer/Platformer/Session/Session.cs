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
        /// The level that the current session is in.
        /// </summary>
        private static Level Level
        {
            get { return (singleton == null ? null : singleton.level); }
            set { Level = value; }
        }


        /// <summary>
        /// Load the level at levelIndex
        /// </summary>
        private static void LoadLevel(int levelIndex)
        {
            Level = new Level(ScreenManager.Game.Content, levelIndex, ScreenManager.GraphicsDevice.Viewport);
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


        // HUD information
        private SpriteFont hudFont;

        private Texture2D winOverlay;
        private Texture2D loseOverlay;
        private Texture2D diedOverlay;

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
        }


        #endregion


        #region Updating


        /// <summary>
        /// Update the session for this frame.
        /// </summary>
        /// <remarks>This should only be called if there are no menus in use.</remarks>
        public static void Update(GameTime gameTime)
        {
            // check the singleton
            if (singleton == null)
            {
                return;
            }
    
            Level.Update(gameTime);
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draws the session environment to the screen
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = singleton.screenManager.SpriteBatch;

            Level.Draw(gameTime, spriteBatch);
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

            if (level.TimeRemaining == TimeSpan.Zero)
            {
                if (level.ReachedExit)
                {
                    status = winOverlay;
                }
                else
                {
                    status = loseOverlay;
                }
            }
            else if (!level.Player.IsAlive)
            {
                status = diedOverlay;
            }


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


        #region Starting a New Session


        /// <summary>
        /// Start a new session based on the data provided.
        /// </summary>
        public static void StartNewSession(int levelIndex, ScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameters
            if (levelIndex == -1)
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

            // set up the initial level
            LoadLevel(levelIndex);
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

                // pop the music
                //AudioManager.PopMusic();

                // clear the singleton
                singleton = null;

                if (gameplayScreen != null)
                {
                    gameplayScreen.ExitScreen();
                }
            }
        }


        #endregion

        
        #region Loading a Session

        /*
        /// <summary>
        /// Start a new session, using the data in the given save game.
        /// </summary>
        /// <param name="saveGameDescription">The description of the save game.</param>
        /// <param name="screenManager">The ScreenManager for the new session.</param>
        public static void LoadSession(SaveGameDescription saveGameDescription,
            ScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameters
            if (saveGameDescription == null)
            {
                throw new ArgumentNullException("saveGameDescription");
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

            // create the new session
            singleton = new Session(screenManager, gameplayScreen);

            // get the storage device and load the session
            GetStorageDevice(
                delegate(StorageDevice storageDevice)
                {
                    LoadSessionResult(storageDevice, saveGameDescription);
                });
        }


        /// <summary>
        /// Receives the storage device and starts a new session, 
        /// using the data in the given save game.
        /// </summary>
        /// <remarks>The new session is created in LoadSessionResult.</remarks>
        /// <param name="storageDevice">The chosen storage device.</param>
        /// <param name="saveGameDescription">The description of the save game.</param>
        public static void LoadSessionResult(StorageDevice storageDevice,
            SaveGameDescription saveGameDescription)
        {
            // check the parameters
            if (saveGameDescription == null)
            {
                throw new ArgumentNullException("saveGameDescription");
            }
            // check the parameter
            if ((storageDevice == null) || !storageDevice.IsConnected)
            {
                return;
            }

            // open the container
            using (StorageContainer storageContainer = OpenContainer(storageDevice))
            {
                using (Stream stream =
                    storageContainer.OpenFile(saveGameDescription.FileName, FileMode.Open))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stream))
                    {
                        // <rolePlayingGameData>
                        xmlReader.ReadStartElement("rolePlayingGameSaveData");

                        // read the map information
                        xmlReader.ReadStartElement("mapData");
                        string mapAssetName =
                            xmlReader.ReadElementString("mapContentName");
                        PlayerPosition playerPosition = new XmlSerializer(
                            typeof(PlayerPosition)).Deserialize(xmlReader)
                            as PlayerPosition;
                        singleton.removedMapChests = new XmlSerializer(
                            typeof(List<WorldEntry<Chest>>)).Deserialize(xmlReader)
                            as List<WorldEntry<Chest>>;
                        singleton.removedMapFixedCombats = new XmlSerializer(
                            typeof(List<WorldEntry<FixedCombat>>)).Deserialize(xmlReader)
                            as List<WorldEntry<FixedCombat>>;
                        singleton.removedMapPlayerNpcs = new XmlSerializer(
                            typeof(List<WorldEntry<Player>>)).Deserialize(xmlReader)
                            as List<WorldEntry<Player>>;
                        singleton.modifiedMapChests = new XmlSerializer(
                            typeof(List<ModifiedChestEntry>)).Deserialize(xmlReader)
                            as List<ModifiedChestEntry>;
                        ChangeMap(mapAssetName, null);
                        TileEngine.PartyLeaderPosition = playerPosition;
                        xmlReader.ReadEndElement();

                        // read the quest information
                        ContentManager content = Session.ScreenManager.Game.Content;
                        xmlReader.ReadStartElement("questData");
                        singleton.questLine = content.Load<QuestLine>(
                            xmlReader.ReadElementString("questLineContentName")).Clone()
                            as QuestLine;
                        singleton.currentQuestIndex = Convert.ToInt32(
                            xmlReader.ReadElementString("currentQuestIndex"));
                        for (int i = 0; i < singleton.currentQuestIndex; i++)
                        {
                            singleton.questLine.Quests[i].Stage =
                                Quest.QuestStage.Completed;
                        }
                        singleton.removedQuestChests = new XmlSerializer(
                            typeof(List<WorldEntry<Chest>>)).Deserialize(xmlReader)
                            as List<WorldEntry<Chest>>;
                        singleton.removedQuestFixedCombats = new XmlSerializer(
                            typeof(List<WorldEntry<FixedCombat>>)).Deserialize(xmlReader)
                            as List<WorldEntry<FixedCombat>>;
                        singleton.modifiedQuestChests = new XmlSerializer(
                            typeof(List<ModifiedChestEntry>)).Deserialize(xmlReader)
                            as List<ModifiedChestEntry>;
                        Quest.QuestStage questStage = (Quest.QuestStage)Enum.Parse(
                            typeof(Quest.QuestStage),
                            xmlReader.ReadElementString("currentQuestStage"), true);
                        if ((singleton.questLine != null) && !IsQuestLineComplete)
                        {
                            singleton.quest =
                                singleton.questLine.Quests[CurrentQuestIndex];
                            singleton.ModifyQuest(singleton.quest);
                            singleton.quest.Stage = questStage;
                        }
                        xmlReader.ReadEndElement();

                        // read the party data
                        singleton.party = new Party(new XmlSerializer(
                            typeof(PartySaveData)).Deserialize(xmlReader)
                            as PartySaveData, content);

                        // </rolePlayingGameSaveData>
                        xmlReader.ReadEndElement();
                    }
                }
            }
        }

        */
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
        

        #region Deleting a Save Game
        /*

        /// <summary>
        /// Delete the save game specified by the description.
        /// </summary>
        /// <param name="saveGameDescription">The description of the save game.</param>
        public static void DeleteSaveGame(SaveGameDescription saveGameDescription)
        {
            // check the parameters
            if (saveGameDescription == null)
            {
                throw new ArgumentNullException("saveGameDescription");
            }

            // get the storage device and load the session
            GetStorageDevice(
                delegate(StorageDevice storageDevice)
                {
                    DeleteSaveGameResult(storageDevice, saveGameDescription);
                });
        }


        /// <summary>
        /// Delete the save game specified by the description.
        /// </summary>
        /// <param name="storageDevice">The chosen storage device.</param>
        /// <param name="saveGameDescription">The description of the save game.</param>
        public static void DeleteSaveGameResult(StorageDevice storageDevice,
            SaveGameDescription saveGameDescription)
        {
            // check the parameters
            if (saveGameDescription == null)
            {
                throw new ArgumentNullException("saveGameDescription");
            }
            // check the parameter
            if ((storageDevice == null) || !storageDevice.IsConnected)
            {
                return;
            }

            // open the container
            using (StorageContainer storageContainer =
                OpenContainer(storageDevice))
            {
                storageContainer.DeleteFile(saveGameDescription.FileName);
                storageContainer.DeleteFile("SaveGameDescription" +
                    Path.GetFileNameWithoutExtension(
                        saveGameDescription.FileName).Substring(8) + ".xml");
            }

            // refresh the save game descriptions
            Session.RefreshSaveGameDescriptions();
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


        #region Storage


        /// <summary>
        /// The stored StorageDevice object.
        /// </summary>
        private static StorageDevice storageDevice;

        /// <summary>
        /// The container name used for save games.
        /// </summary>
        public static string SaveGameContainerName = "Platformer";


        /// <summary>
        /// A delegate for receiving StorageDevice objects.
        /// </summary>
        public delegate void StorageDeviceDelegate(StorageDevice storageDevice);

        /// <summary>
        /// Asynchronously retrieve a storage device.
        /// </summary>
        /// <param name="retrievalDelegate">
        /// The delegate called when the device is available.
        /// </param>
        /// <remarks>
        /// If there is a suitable cached storage device, 
        /// the delegate may be called directly by this function.
        /// </remarks>
        public static void GetStorageDevice(StorageDeviceDelegate retrievalDelegate)
        {
            // check the parameter
            if (retrievalDelegate == null)
            {
                throw new ArgumentNullException("retrievalDelegate");
            }

            // check the stored storage device
            if ((storageDevice != null) && storageDevice.IsConnected)
            {
                retrievalDelegate(storageDevice);
                return;
            }

            // the storage device must be retrieved
            if (!Guide.IsVisible)
            {
                // Reset the device
                storageDevice = null;
                StorageDevice.BeginShowSelector(GetStorageDeviceResult, retrievalDelegate);
            }


        }


        /// <summary>
        /// Asynchronous callback to the guide's BeginShowStorageDeviceSelector call.
        /// </summary>
        /// <param name="result">The IAsyncResult object with the device.</param>
        private static void GetStorageDeviceResult(IAsyncResult result)
        {
            // check the parameter
            if ((result == null) || !result.IsCompleted)
            {
                return;
            }

            // retrieve and store the storage device
            storageDevice = StorageDevice.EndShowSelector(result);

            // check the new storage device 
            if ((storageDevice != null) && storageDevice.IsConnected)
            {
                // it passes; call the stored delegate
                StorageDeviceDelegate func = result.AsyncState as StorageDeviceDelegate;
                if (func != null)
                {
                    func(storageDevice);
                }
            }
        }

        /// <summary>
        /// Synchronously opens storage container
        /// </summary>
        private static StorageContainer OpenContainer(StorageDevice storageDevice)
        {
            IAsyncResult result =
                storageDevice.BeginOpenContainer(Session.SaveGameContainerName, null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = storageDevice.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            return container;
        }


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
